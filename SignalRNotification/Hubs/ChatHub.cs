using Microsoft.AspNetCore.SignalR;

namespace SignalRNotification.Hubs
{
    public class ChatHub : Hub
    {
        static Dictionary<string, string> keyConnectionIds = new Dictionary<string, string>();
        public static string? fid = null;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ChatHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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
            /* if (fid != null) {
                 Clients.Clients(fid).SendAsync("Notify", Name, Message).Wait();
             }*/

            // Clients.all
            // Clients.Clients
            // Clients.Group
        }

        public void ScanCompleted(string folderName)
        {
            var imgUrls = GetAllImage(folderName);
            //Clients.All.SendAsync("ScanCompleted", folderName);
            Clients.All.SendAsync("ScanCompleted", imgUrls);
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
    }
}
