using Microsoft.AspNetCore.SignalR;
public class ProductViewHub : Hub
{
    // Example:
    public async Task SendProductView(string productId, int views)
    {
        await Clients.All.SendAsync("ReceiveProductView", productId, views);
    }
}