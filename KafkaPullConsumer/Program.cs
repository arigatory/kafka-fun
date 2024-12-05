using System.Text.Json;
using Confluent.Kafka;

Console.WriteLine("Welcome to Kafka Pull Consumer!");

string bootstrapServers = "localhost:9094";
string topic = "my-first-topic";

var config = new ConsumerConfig
{
    BootstrapServers = bootstrapServers,
    GroupId = "pull-consumer-group",
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false,  // Disable auto-commit for manual offset management
    FetchMinBytes = 1024
};

using var consumer = new ConsumerBuilder<Ignore, KafkaMessage>(config)
    .SetValueDeserializer(new JsonDeserializer<KafkaMessage>())
    .Build();

consumer.Subscribe(topic);

while (true)
{
    try
    {
        var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));
        if (consumeResult is { Message.Value: var value })
        {
            Console.WriteLine($"Pull Consumer: {JsonSerializer.Serialize(value)} | Partition: {consumeResult.Partition} | Offset: {consumeResult.Offset}");

            // Manually commit the offset
            try
            {
                consumer.Commit(consumeResult);
                Console.WriteLine($"Offset {consumeResult.Offset} committed for partition {consumeResult.Partition}");
            }
            catch (KafkaException e)
            {
                Console.WriteLine($"Error committing offset: {e.Error.Reason}");
            }
        }
    }
    catch (ConsumeException e)
    {
        Console.WriteLine($"Pull Consumer error: {e.Error.Reason}");
    }
    catch (JsonException e)
    {
        Console.WriteLine($"Pull Consumer deserialization error: {e.Message}");
    }
}


// Message class
public class KafkaMessage
{
    public int Id { get; set; }
    public string Content { get; set; } = "";
    public DateTime Timestamp { get; set; }
}

// Serializer
public class JsonSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data);
    }
}

// Deserializer
public class JsonDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return JsonSerializer.Deserialize<T>(data)!;
    }
}