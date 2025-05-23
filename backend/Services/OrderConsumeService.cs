using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection; 
using Entities;
using Context;

public class OrderConsumerService : IHostedService
{
    private readonly string _bootstrapServers;
    private readonly string _topicConsume = "order-events";
    private readonly string _topicProduce = "processed-orders";
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IProducer<Null, string> _producer;
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderConsumerService(IConfiguration config, IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
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

                        var order = JsonSerializer.Deserialize<OrderDto>(consumeResult.Message.Value);
                        bool mailSent = SendMail(order);

                        var resultMessage = new
                        {
                            order.OrderNumber,
                            Status = mailSent ? "Mail gönderildi" : "Mail gönderilemedi",
                            Timestamp = DateTime.UtcNow
                        };

                        var jsonResult = JsonSerializer.Serialize(resultMessage);

                        _producer.Produce(_topicProduce, new Message<Null, string> { Value = jsonResult }, deliveryReport =>
                        {
                            if (deliveryReport.Error.IsError)
                                Console.WriteLine($"Mesaj gönderim hatası: {deliveryReport.Error.Reason}");
                            else
                                Console.WriteLine($"Sonuç mesajı gönderildi: {deliveryReport.TopicPartitionOffset}");
                        });

                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<eCommerceContext>();

                            dbContext.ProcessedOrders.Add(new ProcessedOrders
                            {
                                OrderNumber = order.OrderNumber,
                                Status = mailSent ? "Mail gönderildi" : "Mail gönderilemedi",
                                CreatedDate = DateTime.UtcNow
                            });

                            await dbContext.SaveChangesAsync();
                        }

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

    private bool SendMail(OrderDto orderDto)
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
