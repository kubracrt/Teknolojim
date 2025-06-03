using System.ComponentModel.DataAnnotations;
using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SharedModel.Dto;
using System.Text.Json;
using System.Net;

public class OrderViewConsumerService : IHostedService
{
    private readonly string _topicName = "user-view-events";
    private readonly IHubContext<ProductViewHub> _hubContext;
    private readonly IConsumer<Ignore, string> _consumer;

    public OrderViewConsumerService(IConfiguration config, IHubContext<ProductViewHub> hubContext)
    {
        _hubContext = hubContext;
        var bootstrapServers = config.GetValue<string>("Kafka:BootstrapServers") ?? "localhost:9094";

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "order-view-consumer-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => StartConsuming(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    private async Task StartConsuming(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topicName);
        try
        {
            Console.WriteLine($"Tüketici {_topicName} konusuna abone oldu.");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(cancellationToken);
                    if (result != null)
                    {
                        Console.WriteLine($"Mesaj Alındı: {result.Message.Value}");

                        var productViewEvent = JsonSerializer.Deserialize<ProductViewEvent>(result.Message.Value);                         
                        Console.WriteLine("SignalR mesaj gönderiliyor...");
                        if (productViewEvent != null)
                        {
                            await _hubContext.Clients.All.SendAsync("ReceiveProductView", productViewEvent, cancellationToken);
                            Console.WriteLine("SignalR mesajı gönderildi. ReceiveProductView: " + productViewEvent.ProductId,productViewEvent.ProductName,productViewEvent.ViewedAt);

                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Konu tüketilirken hata oluştu: {ex.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _consumer.Close();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

}