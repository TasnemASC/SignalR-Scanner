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
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }


                // Generate a unique file name
                //  string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string fileName = image.FileName;// Path.GetExtension(image.FileName);

                // Combine the directory path with the file name
                string filePath = Path.Combine(directoryPath, fileName);

                // Save the file to the server
                using (FileStream stream = new(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Optionally, return the URL or path of the saved image
                string imageUrl = Url.Content($"~/images/{folderName}/" + fileName);
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllImage")]
        public async Task<IActionResult> GetAllImage(string folderName)
        {
            string directoryPath = $"wwwroot/images/{folderName}";
            try
            {
                // Get the list of image file names
                string[] imageFileNames = Directory.GetFiles(directoryPath)
                                                   .Select(Path.GetFileName)
                                                    .ToArray();
                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                List<string> imageUrls = imageFileNames.Select(fileName => $"{baseUrl}/images/{folderName}/{fileName}").ToList();

                // Return the list of image file names
                return Ok(imageUrls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
