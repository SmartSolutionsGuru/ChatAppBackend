using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ChatApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VoiceNotesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<VoiceNotesController> _logger;

        public VoiceNotesController(
            IWebHostEnvironment env, 
            ILogger<VoiceNotesController> logger)
        {
            _env = env;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<VoiceNoteUploadResponse>> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // Validate file type (check if content type starts with allowed types to handle codecs)
            var allowedTypes = new[] { "audio/webm", "audio/ogg", "audio/mp4", "audio/mpeg", "audio/wav" };
            var contentType = file.ContentType.Split(';')[0].Trim(); // Remove codec info like ";codecs=opus"
            if (!allowedTypes.Contains(contentType))
                return BadRequest($"Invalid file type: {file.ContentType}");

            // Validate file size (max 5MB for voice notes)
            const long maxSize = 5 * 1024 * 1024;
            if (file.Length > maxSize)
                return BadRequest("File too large. Maximum size is 5MB");

            try
            {
                // Create voice notes directory
                var uploadsDir = Path.Combine(_env.ContentRootPath, "uploads", "voice-notes");
                Directory.CreateDirectory(uploadsDir);

                // Generate unique filename
                var extension = GetExtensionFromContentType(file.ContentType);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsDir, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generate URL
                var url = $"/api/voicenotes/{fileName}";

                _logger.LogInformation("Voice note uploaded: {FileName}, Size: {Size}", fileName, file.Length);

                return Ok(new VoiceNoteUploadResponse
                {
                    Url = url,
                    FileName = fileName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading voice note");
                return StatusCode(500, "Error uploading voice note");
            }
        }

        [HttpGet("{fileName}")]
        [AllowAnonymous] // Allow fetching voice notes without auth for audio playback
        public IActionResult Get(string fileName)
        {
            var filePath = Path.Combine(_env.ContentRootPath, "uploads", "voice-notes", fileName);
            
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var contentType = GetContentTypeFromFileName(fileName);
            return PhysicalFile(filePath, contentType);
        }

        private static string GetExtensionFromContentType(string contentType) => contentType switch
        {
            "audio/webm" => ".webm",
            "audio/ogg" => ".ogg",
            "audio/mp4" => ".m4a",
            "audio/mpeg" => ".mp3",
            "audio/wav" => ".wav",
            _ => ".webm"
        };

        private static string GetContentTypeFromFileName(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".webm" => "audio/webm",
                ".ogg" => "audio/ogg",
                ".m4a" => "audio/mp4",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                _ => "application/octet-stream"
            };
        }
    }

    public class VoiceNoteUploadResponse
    {
        public string Url { get; set; } = default!;
        public string FileName { get; set; } = default!;
    }
}
