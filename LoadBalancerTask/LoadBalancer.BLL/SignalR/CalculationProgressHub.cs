using Microsoft.AspNetCore.SignalR;

namespace LoadBalancer.BLL.SignalR
{
    public class CalculationProgressHub : Hub
    {
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public async Task SendUpdateProgress(string userId, double progress)
        {
            Console.WriteLine($"Received progress update for user {userId}: {progress}");
            // await Clients.All.SendAsync("UpdateProgress", progress);
            await Clients.Client(Context.ConnectionId).SendAsync("UpdateProgress", progress);

        }
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendProgress(string groupName, double progress)
        {
            Console.WriteLine($"Received progress update for groupName {groupName}: {progress}");
            await Clients.Group(groupName).SendAsync("UpdateProgress", progress);
            //await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);

        }
    }
}
