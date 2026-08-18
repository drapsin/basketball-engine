using Microsoft.AspNetCore.Http;

namespace nba_mvc.Services.Image
{
    public class LocalImageUploader : IImageUploader
    {
        private readonly IWebHostEnvironment _env;

        public LocalImageUploader(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var folderPath = Path.Combine(_env.WebRootPath, "image");

            Directory.CreateDirectory(folderPath);

            var savePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return "/image/" + fileName;
        }
    }
}