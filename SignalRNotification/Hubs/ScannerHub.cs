using Microsoft.AspNetCore.SignalR;

namespace SignalRNotification.Hubs
{
    public class ScannerHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ScannerHub(IHttpContextAccessor httpContextAccessor,
                          IWebHostEnvironment webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public void SendMessage()
        {
            string folderName = Guid.NewGuid().ToString();

            // Create the folder
            Directory.CreateDirectory($"wwwroot/images/{folderName}");
            Clients.All.SendAsync("startscan", $"{folderName}");
        }

        public void ImageUploaded(string ImageName)
        {
            var request = _httpContextAccessor.HttpContext.Request;

            string baseUrl = $"{request.Scheme}://{request.Host}";
            string imagePath = $"{baseUrl}/images/{ImageName}";

            Clients.All.SendAsync("ImageUploaded", $"{imagePath}");
        }

        public void ScanCompleted(string folderName)
        {
            Clients.All.SendAsync("ScanCompleted", folderName);
        }
    }
}
