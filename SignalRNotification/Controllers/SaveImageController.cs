using Microsoft.AspNetCore.Mvc;

namespace SignalRNotification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaveImageController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image, [FromForm] string folderName)
        {
            try
            {
                // Check if the image file is provided
                if (image == null || image.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // Specify the directory where you want to save the image
                string directoryPath = $"wwwroot/images/{folderName}"; // Adjust the path as needed

                // Create the directory if it doesn't exist
                Directory.CreateDirectory(directoryPath);

                // Generate a unique file name
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                // Combine the directory path with the file name
                string filePath = Path.Combine(directoryPath, fileName);

                // Save the file to the server
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Optionally, return the URL or path of the saved image
                string imageUrl = Url.Content("~/images/" + fileName);
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
