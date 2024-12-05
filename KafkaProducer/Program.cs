using System.Text.Json;
using Confluent.Kafka;

Console.WriteLine("Welcome to Kafka Producer!");

string bootstrapServers = "localhost:9094";
string topic = "my-first-topic";

var config = new ProducerConfig
{
    BootstrapServers = bootstrapServers,
    Acks = Acks.All,  // Ensure "at least once" delivery
    EnableIdempotence = true,  // Avoid duplicate messages
    MessageSendMaxRetries = 3,  // Retry on temporary failures
    RetryBackoffMs = 1000  // Wait 1 second between retries
};
using var producer = new ProducerBuilder<Null, KafkaMessage>(config)
    .SetValueSerializer(new JsonSerializer<KafkaMessage>())
    .Build();

for (int i = 0; i < 100; i++)
{
    var message = new KafkaMessage
    {
        Id = i,
        Content = $"Message {i}",
        Timestamp = DateTime.UtcNow
    };

    try
    {
        var deliveryResult = await producer.ProduceAsync(topic, new Message<Null, KafkaMessage> { Value = message });
        Console.WriteLine($"Produced: {JsonSerializer.Serialize(message)}");
    }
    catch (ProduceException<Null, KafkaMessage> e)
    {
        Console.WriteLine($"Delivery failed: {e.Error.Reason}");
    }

    await Task.Delay(100); // Simulate some work
}

public class KafkaMessage
{
    public int Id { get; set; }
    public string Content { get; set; } = "";
    public DateTime Timestamp { get; set; }
}

public class JsonSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data);
    }
}