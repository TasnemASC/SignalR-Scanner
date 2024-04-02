using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;

namespace SignalRNotification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaveImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SaveImageController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
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

        [HttpGet("GeneratePdf")]
        public IActionResult GeneratePdf(string folder)
        {
            string wwwrootPath = _webHostEnvironment.WebRootPath;
            string imagesDirectory = Path.Combine(wwwrootPath, "images");
            imagesDirectory = Path.Combine(imagesDirectory, folder);
            // Get image files
            string[] imageFiles = Directory.GetFiles(imagesDirectory);

            // Create PDF document
            /* using (MemoryStream memoryStream = new())
             {
                 Document document = new();
                 PdfWriter.GetInstance(document, memoryStream);
                 document.Open();

                 // Add each image as a page in the PDF document
                 foreach (string imageFile in imageFiles)
                 {
                     Image image = Image.GetInstance(imageFile);
                     document.Add(image);
                 }

                 document.Close();

                 // Return PDF file as byte array
                 return File(memoryStream.ToArray(), "application/pdf", "images.pdf");
             }*/
            using (FileStream fs = new(Path.Combine(imagesDirectory, "images.pdf"), FileMode.Create))
            {
                Document document = new();
                PdfWriter.GetInstance(document, fs);
                document.Open();
                float width = 0;
                float height = 0;


                // Add each image as a page in the PDF document
                foreach (string imageFile in imageFiles)
                {
                    document.NewPage();
                    Image image = Image.GetInstance(imageFile);
                    // image.Width = 8.5;
                    image.SetAbsolutePosition(0, 0);
                    image.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
                    document.Add(image);
                }

                document.Close();
            }

            return Ok(Path.Combine(imagesDirectory, "images.pdf"));
        }
    }
}
