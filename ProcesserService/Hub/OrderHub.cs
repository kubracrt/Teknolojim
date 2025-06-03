using Microsoft.AspNetCore.SignalR;
public class OrderHub : Hub
{
    // You can add methods here that clients can call,
    // or methods that the server calls to send messages to clients.
    // Example:
    public async Task SendOrder(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveOrder", user, message);
    }
}
