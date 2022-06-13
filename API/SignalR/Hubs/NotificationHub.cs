using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalR.Models;

namespace SignalR.Hubs
{
    public class NotificationHub : Hub
    {
        public Task NotifyAll(Notification notification)
        {
            return Clients.All.SendAsync("NotificationReceived", notification);
        }
    }
}