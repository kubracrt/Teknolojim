using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromMemory(
        new[]
        {
            new Yarp.ReverseProxy.Configuration.RouteConfig()
{
    RouteId = "signalr_views_route",
    ClusterId = "signalr_cluster",
    Match = new Yarp.ReverseProxy.Configuration.RouteMatch()
    {
        Path = "/productviewhub/{**catch-all}",
        Methods = new[] { "GET", "POST", "OPTIONS" }
    },
    Transforms = Array.Empty<Dictionary<string, string>>()
},
           new Yarp.ReverseProxy.Configuration.RouteConfig()
{
    RouteId = "signalr_orders_route",
    ClusterId = "signalr_cluster",
    Match = new Yarp.ReverseProxy.Configuration.RouteMatch()
    {
        Path = "/orderhub/{**catch-all}",
        Methods = new[] { "GET", "POST", "OPTIONS" }
    },
    Transforms = Array.Empty<Dictionary<string, string>>()
},

            new Yarp.ReverseProxy.Configuration.RouteConfig()
            {
                RouteId = "api_route",
                ClusterId = "orders_cluster",
                Match = new Yarp.ReverseProxy.Configuration.RouteMatch()
                {
                    Path = "/api/{**catch-all}",
                    Methods = new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" }
                }
            }
        },
        new[]
        {
            new Yarp.ReverseProxy.Configuration.ClusterConfig()
            {
                ClusterId = "signalr_cluster",
                Destinations = new Dictionary<string, Yarp.ReverseProxy.Configuration.DestinationConfig>
                {
                    { "signalr_destination", new Yarp.ReverseProxy.Configuration.DestinationConfig { Address = "http://localhost:5222/" } }
                }
            },
            new Yarp.ReverseProxy.Configuration.ClusterConfig()
            {
                ClusterId = "orders_cluster",
                Destinations = new Dictionary<string, Yarp.ReverseProxy.Configuration.DestinationConfig>
                {
                    { "orders_destination", new Yarp.ReverseProxy.Configuration.DestinationConfig { Address = "http://localhost:5248/" } }
                }
            }
        });

var app = builder.Build();

app.UseWebSockets();

app.MapReverseProxy();

app.Run();
