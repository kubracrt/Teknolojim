using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SharedModel.Dto;
using System.Text.Json;
using System.Net.Mail;
using System.Net;

public class OrderConsumerService : IHostedService
{
    private readonly string _bootstrapServers;
    private readonly string _topicConsume = "order-events";
    private readonly string _topicProduce = "processed-orders";

    private readonly IHubContext<OrderHub> _hubContext;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IProducer<Null, string> _producer;
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderConsumerService(IConfiguration config, IServiceScopeFactory scopeFactory, IHubContext<OrderHub> hubContext)
    {
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
        _bootstrapServers = config.GetValue<string>("Kafka:BootstrapServers") ?? "localhost:9094";

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "order-consumer-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _bootstrapServers
        };

        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => StartConsuming(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    public async Task StartConsuming(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topicConsume);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    if (consumeResult != null)
                    {
                        Console.WriteLine($"Mesaj alındı: {consumeResult.Message.Value}");

                        var order = JsonSerializer.Deserialize<Order>(consumeResult.Message.Value);
                        Console.WriteLine($"Sipariş alındı: {order.OrderNumber}, Kullanıcı: {order.UserName}, Fiyat: {order.Price}");
                        bool mailSent = SendMail(order);

                        var resultMessage = new
                        {
                            order.OrderNumber,
                            Status = mailSent ? "Mail gönderildi" : "Mail gönderilemedi",
                            Timestamp = DateTime.UtcNow
                        };
                        
                        Console.WriteLine("SignalR mesajı gönderiliyor...");
                        try
                        {
                            await _hubContext.Clients.All.SendAsync("ReceiveOrder", resultMessage);
                            Console.WriteLine("SignalR mesajı gönderildi. receivedOrder: " + resultMessage);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"SignalR mesaj gönderim hatası: {ex.Message}");
                        }


                        var jsonResult = JsonSerializer.Serialize(resultMessage);

                        _producer.Produce(_topicProduce, new Message<Null, string> { Value = jsonResult }, deliveryReport =>
                        {
                            if (deliveryReport.Error.IsError)
                                Console.WriteLine($"Mesaj gönderim hatası: {deliveryReport.Error.Reason}");
                            else
                                Console.WriteLine($"Sonuç mesajı gönderildi: {deliveryReport.TopicPartitionOffset}");
                        });

                        _consumer.Commit(consumeResult);
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume hatası: {e.Error.Reason}");
                }
                catch (JsonException je)
                {
                    Console.WriteLine($"JSON deserialize hatası: {je.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Genel hata: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Tüketici durduruldu.");
        }
        finally
        {
            _consumer.Close();
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private bool SendMail(Order orderDto)
    {
        try
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("kubracorutt59@gmail.com", "tvrzgcglrfqqfcbb"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("kubracorutt59@gmail.com"),
                Subject = "Siparişiniz Alındı",
                Body = $"{orderDto.UserName} {orderDto.Price} TL'lik siparişiniz başarıyla alındı. Teşekkürler!",
                IsBodyHtml = false,
            };
            mailMessage.To.Add(orderDto.Usermail);

            smtpClient.Send(mailMessage);
            Console.WriteLine($"Mail gönderildi: {orderDto.Usermail}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Mail gönderme hatası: {ex.Message}");
            return false;
        }
    }
}



