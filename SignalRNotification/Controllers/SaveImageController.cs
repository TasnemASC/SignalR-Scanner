using Microsoft.AspNetCore.Mvc;

namespace SignalRNotification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaveImageController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadImage(string folderName)
        {
            try
            {
                // Check if the request contains a file
                if (Request.Form.Files.Count == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // Get the uploaded file
                var file = Request.Form.Files[0];

                // Validate the file type and size if needed

                // Process the image data (e.g., resize, convert format)

                // Save the image to a storage location (e.g., local file system)
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine($"wwwroot/images/{folderName}", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Optionally, return the URL or path of the saved image
                var imageUrl = Url.Content("~/images/" + fileName);
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
