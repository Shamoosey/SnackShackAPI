using Azure.Core;
using Microsoft.AspNetCore.SignalR;

namespace SnackShackAPI.SignalR
{
    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // your logic to fetch a user identifier goes here.

            // for example:
            Console.WriteLine(connection.User.Identity.Name);
            //var userId = MyCustomUserClass.FindUserId(connection.User.Identity.Name);
            return connection.User.Identity.Name;
        }
    }
}
