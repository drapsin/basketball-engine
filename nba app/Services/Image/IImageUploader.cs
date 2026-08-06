using Microsoft.AspNetCore.Http;

namespace nba_mvc.Services.Image
{
    public interface IImageUploader
    {
        Task<string> UploadImageAsync(IFormFile image);
    }
}