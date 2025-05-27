
using System.Text.Json;
using Confluent.Kafka;
using Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

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

                        var productViewEvent = JsonSerializer.Deserialize<ProductViewEvent>(
                            result.Message.Value,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                        Console.WriteLine("SignalR mesaj gönderiliyor...");
                        if (productViewEvent != null)
                        {
                            try
                            {
                                await _hubContext.Clients.All.SendAsync("ReceiveViewEvent", productViewEvent);
                                Console.WriteLine("SignalR ile mesaj gönderildi.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"SignalR Hatası: {ex.Message}\n{ex.StackTrace}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Mesaj deserialize edilemedi.");
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Kafka Consume hatası: {ex.Error.Reason}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Tüketici çalıştırılırken hata oluştu: {ex.Message}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Close();
        return Task.CompletedTask;
    }
}
