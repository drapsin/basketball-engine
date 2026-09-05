using Microsoft.EntityFrameworkCore;
using nba_mvc.Data;
using nba_mvc.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace nba_mvc.Services.ExternalData
{
    public class NbaImportService : INbaImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public NbaImportService(ApplicationDbContext context, HttpClient http, IConfiguration config)
        {
            _context = context;
            _http = http;
            _config = config;

            _http.BaseAddress = new Uri("https://api.balldontlie.io/nba/v1/");
            _http.DefaultRequestHeaders.Add("Authorization", _config["BallDontLie:ApiKey"]);
        }

        private static readonly HashSet<string> RealNbaAbbreviations = new()
        {
            "ATL", "BOS", "BKN", "CHA", "CHI", "CLE", "DAL", "DEN", "DET", "GSW",
            "HOU", "IND", "LAC", "LAL", "MEM", "MIA", "MIL", "MIN", "NOP", "NYK",
            "OKC", "ORL", "PHI", "PHX", "POR", "SAC", "SAS", "TOR", "UTA", "WAS"
        };

        public async Task<(int teamsImported, int playersImported)> ImportRealNbaDataAsync()
        {
            // Wipe existing games/stats/rosters so the real import starts clean.
            _context.ActionEvent.RemoveRange(_context.ActionEvent);
            _context.Game.RemoveRange(_context.Game);
            _context.Player.RemoveRange(_context.Player);
            _context.Coach.RemoveRange(_context.Coach);
            _context.Team.RemoveRange(_context.Team);
            await _context.SaveChangesAsync();

            var arenaIds = await _context.Arena.Select(a => a.Id).ToListAsync();
            if (arenaIds.Count == 0)
                throw new InvalidOperationException("No arenas exist to assign to imported teams.");

            var teamsResponse = await GetAsync<BdlListResponse<BdlTeam>>("teams");
            await Task.Delay(TimeSpan.FromSeconds(13)); // stay under 5 req/min

            var realTeams = teamsResponse.Data
                .Where(t => RealNbaAbbreviations.Contains(t.Abbreviation.ToUpper()))
                .ToList();

            var teamIdMap = new Dictionary<int, Guid>();
            var rnd = new Random();
            var teamsImported = 0;

            foreach (var bdlTeam in realTeams)
            {
                var team = new Models.Team
                {
                    Id = Guid.NewGuid(),
                    Name = bdlTeam.Name,
                    City = bdlTeam.City,
                    Site = $"https://www.nba.com/{bdlTeam.Abbreviation.ToLower()}",
                    Sponsor = "N/A",
                    News = $"{bdlTeam.FullName} official roster.",
                    Ranking = "",
                    Contact = "N/A",
                    Conference = bdlTeam.Conference == "East" ? Conference.Eastern : Conference.Western,
                    Division = MapDivision(bdlTeam.Division),
                    ArenaId = arenaIds[rnd.Next(arenaIds.Count)],
                    ImageUrl = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(bdlTeam.Abbreviation)}&background=random&bold=true"
                };

                _context.Team.Add(team);
                teamIdMap[bdlTeam.Id] = team.Id;
                teamsImported++;

                // One placeholder coach per real team — BallDontLie has no coach data.
                _context.Coach.Add(new Models.Coach
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Head",
                    LastName = $"Coach ({bdlTeam.Abbreviation})",
                    Age = rnd.Next(40, 65),
                    History = $"Head coach of the {bdlTeam.FullName}.",
                    TeamId = team.Id,
                    ImageUrl = null
                });
            }

            await _context.SaveChangesAsync();

            var playersImported = 0;

            foreach (var bdlTeam in realTeams)
            {
                await Task.Delay(TimeSpan.FromSeconds(13)); // stay under 5 req/min

                var playersResponse = await GetAsync<BdlListResponse<BdlPlayer>>($"players?team_ids[]={bdlTeam.Id}&per_page=25");

                foreach (var bdlPlayer in playersResponse.Data)
                {
                    if (!teamIdMap.TryGetValue(bdlTeam.Id, out var localTeamId)) continue;

                    var fullName = $"{bdlPlayer.FirstName} {bdlPlayer.LastName}";

                    _context.Player.Add(new Models.Player
                    {
                        Id = Guid.NewGuid(),
                        FirstName = bdlPlayer.FirstName,
                        LastName = bdlPlayer.LastName,
                        Age = rnd.Next(19, 38),
                        Position = string.IsNullOrWhiteSpace(bdlPlayer.Position) ? "G" : bdlPlayer.Position,
                        TeamId = localTeamId,
                        Height = rnd.Next(180, 215),
                        Weight = rnd.Next(75, 125),
                        Agent = "N/A",
                        Sponsor = "N/A",
                        News = $"{fullName} — official roster entry.",
                        ImageUrl = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(fullName)}&background=random"
                    });

                    playersImported++;
                }
            }

            await _context.SaveChangesAsync();

            return (teamsImported, playersImported);
        }

        private static Division MapDivision(string bdlDivision) => bdlDivision switch
        {
            "Atlantic" => Division.Atlantic,
            "Central" => Division.Central,
            "Southeast" => Division.Southeast,
            "Northwest" => Division.Northwest,
            "Pacific" => Division.Pacific,
            "Southwest" => Division.Southwest,
            _ => Division.Atlantic
        };

        private async Task<T> GetAsync<T>(string path)
        {
            var response = await _http.GetAsync(path);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        private class BdlListResponse<T>
        {
            [JsonPropertyName("data")]
            public List<T> Data { get; set; } = new();
        }

        private class BdlTeam
        {
            [JsonPropertyName("id")] public int Id { get; set; }
            [JsonPropertyName("conference")] public string Conference { get; set; } = "";
            [JsonPropertyName("division")] public string Division { get; set; } = "";
            [JsonPropertyName("city")] public string City { get; set; } = "";
            [JsonPropertyName("name")] public string Name { get; set; } = "";
            [JsonPropertyName("full_name")] public string FullName { get; set; } = "";
            [JsonPropertyName("abbreviation")] public string Abbreviation { get; set; } = "";
        }

        private class BdlPlayer
        {
            [JsonPropertyName("first_name")] public string FirstName { get; set; } = "";
            [JsonPropertyName("last_name")] public string LastName { get; set; } = "";
            [JsonPropertyName("position")] public string Position { get; set; } = "";
        }
    }
}