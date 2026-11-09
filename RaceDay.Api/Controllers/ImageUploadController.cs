using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/images")]

    public class ImageUploadController : ControllerBase
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly RacedayDbContext _context; // Your EF database context

        public ImageUploadController(BlobServiceClient blobServiceClient, RacedayDbContext context)
        {
            _blobServiceClient = blobServiceClient;
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null) return BadRequest("File is empty");

            // 1. Get reference to your Azure Container
            var containerClient = _blobServiceClient.GetBlobContainerClient("raceday-uploads");
            await containerClient.CreateIfNotExistsAsync();

            // 2. Generate a unique name to prevent overwrites
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            // 3. Stream the file directly up to Azure Blob Storage
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            // 4. Construct the public or private Blob URL
            string blobUrl = blobClient.Uri.ToString();

            // 5. Save that URL string to your local SQL Database record
            var raceImageRecord = new RaceImage { ImageUrl = blobUrl, UploadedAt = DateTime.UtcNow };
            _context.RaceImages.Add(raceImageRecord);
            await _context.SaveChangesAsync();

            // 6. Return the database state back to MVC
            return Ok(raceImageRecord);
        }
    }
}