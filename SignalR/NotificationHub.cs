using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SnackShackAPI.SignalR
{
    //[Authorize]
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", user, message);
        }

        public override Task OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;

            return base.OnConnectedAsync();
        }
    }
}
