using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json; 
using System.Threading.Tasks;
using Confluent.Kafka; 
using Confluent.Kafka.Admin;
using Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;

public class KafkaProducerService
{
    private readonly string _bootstrapServers;
    private readonly IProducer<Null, string> _producer;

    public KafkaProducerService(IConfiguration config)
    {
        _bootstrapServers = "localhost:9094";

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _bootstrapServers
        };

        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();

        CreateTopicsIfNotExistsAsync().GetAwaiter().GetResult();
    }

    private async Task CreateTopicsIfNotExistsAsync()
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = _bootstrapServers
        }).Build();

        var existingTopics = adminClient.GetMetadata(TimeSpan.FromSeconds(10)).Topics.Select(t => t.Topic).ToList();

        var topicsToCreate = new List<TopicSpecification>();

        if (!existingTopics.Contains("order-events"))
        {
            topicsToCreate.Add(new TopicSpecification
            {
                Name = "order-events",
                NumPartitions = 2, 
                ReplicationFactor = 1 
            });
        }

        if (!existingTopics.Contains("user-view-events"))
        {
            topicsToCreate.Add(new TopicSpecification
            {
                Name = "user-view-events",
                NumPartitions = 2,
                ReplicationFactor = 1
            });
        }

        if (!existingTopics.Contains("processed-orders"))
        {
            topicsToCreate.Add(new TopicSpecification
            {
                Name = "processed-orders",
                NumPartitions = 2,
                ReplicationFactor = 1
            });
        }

        if (topicsToCreate.Count == 0)
        {
            Console.WriteLine("Kafka topic'leri zaten mevcut.");
            return;
        }

        try
        {
            await adminClient.CreateTopicsAsync(topicsToCreate);
            Console.WriteLine("Kafka topic'leri başarıyla oluşturuldu:");
            foreach (var topic in topicsToCreate)
            {
                Console.WriteLine($"- {topic.Name}");
            }
        }
        catch (CreateTopicsException ex)
        {
            Console.WriteLine($"Topic oluşturma hatası: {ex.Message}");
        }
    }

    public async Task SendOrderAsync(object orderData)
    {
        try
        {
            var message = JsonSerializer.Serialize(orderData);

            var result = await _producer.ProduceAsync("order-events", new Message<Null, string> { Value = message });

            Console.WriteLine($"Kafka'ya mesaj gönderildi: {result.TopicPartitionOffset}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Kafka mesaj gönderme hatası: {ex.Message}");
        }
    }

    public async Task SendProductViewAsync(string productId, string productName, string productImageUrl)
    {
        try
        {
            var viewEvent = new ProductViewEvent
            {
                
                ProductId = productId,
                ProductName = productName,
                ProductImageUrl = productImageUrl,
                ViewedAt = DateTime.UtcNow
            };

            var jsonResult = JsonSerializer.Serialize(viewEvent);
            var result = await _producer.ProduceAsync("user-view-events", new Message<Null, string> { Value = jsonResult });

            Console.WriteLine($"Kullanıcı görüntüleme olayı Kafka'ya gönderildi: {result.TopicPartitionOffset}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Kafka ürün görüntülenme gönderme hatası: {ex.Message}");
        }
        
    }
}
