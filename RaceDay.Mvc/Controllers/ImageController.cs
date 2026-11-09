using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers
{
    public class ImageController: Controller
    {
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file selected.");

            using var client = new HttpClient();
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();

           
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.FileName);

         
            var response = await client.PostAsync("https://localhost:XXXX/api/images/upload", content);

            if (response.IsSuccessStatusCode)
            {
                // Read the returned data (like the database record or saved URL)
                var resultData = await response.Content.ReadAsStringAsync();
                return RedirectToAction("Index");
            }

            return View("Error");
        }

    }
}
