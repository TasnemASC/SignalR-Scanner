using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.SignalR;

namespace SignalRNotification.Hubs
{
    public class ChatHub : Hub
    {
        static Dictionary<string, string> keyConnectionIds = new Dictionary<string, string>();
        public static string? fid = null;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ChatHub(IHttpContextAccessor httpContextAccessor,
                       IWebHostEnvironment webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }

        public override Task OnConnectedAsync()
        {
            if (fid == null)
            {
                fid = Context.ConnectionId;
            }

            keyConnectionIds[Context.ConnectionId] = Context.ConnectionId;
            return base.OnConnectedAsync();
        }

        public void SendMessage()
        {
            string folderName = Guid.NewGuid().ToString();

            // Create the folder
            Directory.CreateDirectory($"wwwroot/images/{folderName}");
            Clients.All.SendAsync("startscan", $"{folderName}");
        }

        public void ScanCompleted(string folderName)
        {
            var pdfUrl = GeneratePdf(folderName);//GetAllImage(folderName);
            //Clients.All.SendAsync("ScanCompleted", folderName);
            Clients.All.SendAsync("ScanCompleted", pdfUrl);
        }

        public List<string> GetAllImage(string folderName)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            string directoryPath = $"wwwroot/images/{folderName}";
            try
            {
                // Get the list of image file names
                string[] imageFileNames = Directory.GetFiles(directoryPath)
                                                   .Select(Path.GetFileName)
                                                    .ToArray();
                string baseUrl = $"{request.Scheme}://{request.Host}";
                List<string> imageUrls = imageFileNames.Select(fileName => $"{baseUrl}/images/{folderName}/{fileName}").ToList();

                // Return the list of image file names
                return imageUrls;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GeneratePdf(string folder)
        {
            string wwwrootPath = _webHostEnvironment.WebRootPath;
            string imagesDirectory = Path.Combine(wwwrootPath, "images");
            imagesDirectory = Path.Combine(imagesDirectory, folder);
            // Get image files
            string[] imageFiles = Directory.GetFiles(imagesDirectory);

            using (FileStream fs = new(Path.Combine(imagesDirectory, "images.pdf"), FileMode.Create))
            {
                Document document = new();
                PdfWriter.GetInstance(document, fs);
                document.Open();

                // Add each image as a page in the PDF document
                foreach (string imageFile in imageFiles)
                {
                    Image image = Image.GetInstance(imageFile);
                    //   image.SetAbsolutePosition(0, 0);
                    //  image.ScaleToFit(document.PageSize.Width,0);
                    document.Add(image);
                }

                document.Close();
            }

            var request = _httpContextAccessor.HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/images/{folder}/images.pdf";
        }
    }
}
