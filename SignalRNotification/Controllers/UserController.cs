using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRNotification.Models;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace SignalRNotification.Controllers
{
    public class UserController : Controller
    {
        //private readonly IHubContext<Product> _hubContext;
        static string fid;
       public UserController()
        {
           // _hubContext = hubContext;
        }
        public ActionResult ChatGroup()
        {
            if (fid != null) {
               // _hubContext.Clients.All.SendAsync("Notify", "new User", "Added").Wait();
            }
                // Clients.Clients(fid).SendAsync("Notify", Name, Message).Wait();
               
            return View();
        }
    }
}
