using Microsoft.AspNetCore.Http;

namespace nba_mvc.Services.Image
{
    public class CloudinaryImageUploader : IImageUploader
    {
        private readonly ICloudinaryService _cloudinaryService;

        public CloudinaryImageUploader(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            return await _cloudinaryService.UploadImageAsync(image);
        }
    }
}