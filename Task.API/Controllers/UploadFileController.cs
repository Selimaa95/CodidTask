using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task.API.Entities;
using File = Task.API.Entities.File;

namespace Task.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UploadFileController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public UploadFileController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("files")]
        public async Task<ActionResult<IEnumerable<File>>> GetFiles()
        {
            var files = await _dbContext.Files.ToListAsync();

            if (files == null || files.Count == 0)
            {
                return NotFound("No files found.");
            }

            return Ok(files);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is not selected or empty.");

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                var fileModel = new File
                {
                    FileName = file.FileName,
                    FileContent = memoryStream.ToArray(),
                    UploadTime = DateTime.Now
                };

                _dbContext.Files.Add(fileModel);
                await _dbContext.SaveChangesAsync();
            }

            return Ok("File uploaded successfully.");
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var fileModel = await _dbContext.Files.FindAsync(id);

            if (fileModel == null)
                return NotFound("File not found.");

            var memoryStream = new MemoryStream(fileModel.FileContent);

            return File(memoryStream, "application/octet-stream", fileModel.FileName);
        }
    }
}
