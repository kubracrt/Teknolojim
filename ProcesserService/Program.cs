// Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR; // Ensure this is included

var builder = WebApplication.CreateBuilder(args);

// Add SignalR services to the container.
builder.Services.AddSignalR();

// Configure CORS policy to allow your Angular development client.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200") // Your Angular app's origin
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Essential for SignalR
    });
});

builder.Services.AddHostedService<OrderConsumerService>();
builder.Services.AddHostedService<OrderViewConsumerService>();

var app = builder.Build();


// Apply the CORS policy.
app.UseCors("AllowAngularDevClient");

// Enable routing for SignalR hubs.
app.UseRouting();

// Enable WebSockets for SignalR.
app.UseWebSockets();

// Map your SignalR hubs to their respective paths.
app.MapHub<OrderHub>("/orderhub");
app.MapHub<ProductViewHub>("/productviewhub");

app.Run();


app.MapHub<ProductViewHub>("/productviewhub");
