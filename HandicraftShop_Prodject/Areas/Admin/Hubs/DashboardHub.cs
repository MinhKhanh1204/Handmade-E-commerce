using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HandicraftShop_Project.Hubs
{
    public class DashboardHub : Hub
    {
        public async Task NotifyUpdate(string type, string message)
        {
            await Clients.All.SendAsync("ReceiveDashboardUpdate", type, message);
        }
    }
}
