using Microsoft.AspNetCore.SignalR;

namespace SignalRNotification.Hubs
{
    public class ChatHub : Hub
    {
        static Dictionary<string, string> keyConnectionIds = new Dictionary<string, string>();
        public static string? fid = null;
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

        public void ScanCompleted()
        {

        }
    }
}
