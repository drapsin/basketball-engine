using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nba_mvc.Services.Image;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageUploader _imageUploader;

        private static readonly string[] AllowedContentTypes =
        {
            "image/jpeg", "image/png", "image/webp", "image/gif"
        };

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public ImagesController(IImageUploader imageUploader)
        {
            _imageUploader = imageUploader;
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file was provided.");

            if (file.Length > MaxFileSizeBytes)
                return BadRequest("File exceeds the 5 MB size limit.");

            if (!AllowedContentTypes.Contains(file.ContentType))
                return BadRequest("Unsupported file type. Allowed: JPEG, PNG, WEBP, GIF.");

            var url = await _imageUploader.UploadImageAsync(file);
            return Ok(new { url });
        }
    }
}