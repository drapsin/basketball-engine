using Microsoft.AspNetCore.Http;

namespace nba_mvc.Services.Image
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}