namespace nba_mvc.Services.ExternalData
{
    public interface INbaImportService
    {
        Task<(int teamsImported, int playersImported)> ImportRealNbaDataAsync();
    }
}