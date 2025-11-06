using Microsoft.AspNetCore.SignalR;

public class ApprovalHub : Hub
{
    // Server -> Client
    public async Task NotifyApproved(string entityType, int entityId)
    {
        await Clients.All.SendAsync("ReceiveApproval", entityType, entityId);
    }
}
