using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using nba_mvc.Data;
using nba_mvc.Repositories.ActionEvent;
using nba_mvc.Repositories.Arena;
using nba_mvc.Repositories.Coach;
using nba_mvc.Repositories.Game;
using nba_mvc.Repositories.Player;
using nba_mvc.Repositories.Referee;
using nba_mvc.Repositories.Team;
using nba_mvc.Services.ActionEvent;
using nba_mvc.Services.Arena;
using nba_mvc.Services.Coach;
using nba_mvc.Services.Game;
using nba_mvc.Services.Image;
using nba_mvc.Services.Player;
using nba_mvc.Services.Referee;
using nba_mvc.Services.Simulation;
using nba_mvc.Services.Stats;
using nba_mvc.Services.Team;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(cfg => { }, typeof(Program));

// Repositories
builder.Services.AddScoped<IArenaRepository, ArenaRepository>();
builder.Services.AddScoped<IRefereeRepository, RefereeRepository>();
builder.Services.AddScoped<ICoachRepository, CoachRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IActionEventRepository, ActionEventRepository>();

// Services
builder.Services.AddScoped<IArenaService, ArenaService>();
builder.Services.AddScoped<IRefereeService, RefereeService>();
builder.Services.AddScoped<ICoachService, CoachService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IActionEventService, ActionEventService>();
builder.Services.AddScoped<IGameStatsService, GameStatsService>();
builder.Services.AddScoped<IGameStatsService, GameStatsService>();
builder.Services.AddScoped<IStandingsService, StandingsService>();

// Images
builder.Services.AddHttpContextAccessor();

// Game Simulation 
builder.Services.AddSingleton<IGameSimulationStateStore, GameSimulationStateStore>();
builder.Services.AddScoped<IGameSimulationEngine, GameSimulationEngine>();

var imageStorage = builder.Configuration["ImageStorage"];
if (imageStorage == "Cloudinary")
{
    builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
    builder.Services.AddScoped<IImageUploader, CloudinaryImageUploader>();
}
else
{
    builder.Services.AddScoped<IImageUploader, LocalImageUploader>();
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.SeedRoles(roleManager);

    var context = services.GetRequiredService<ApplicationDbContext>();
    DbInitializer.SeedData(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();