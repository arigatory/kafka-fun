using System.Text.Json;
using Confluent.Kafka;

Console.WriteLine("Welcome to Kafka Push Consumer!");

string bootstrapServers = "localhost:9094";
string topic = "my-first-topic";


var config = new ConsumerConfig
{
    BootstrapServers = bootstrapServers,
    GroupId = "push-consumer-group",
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = true,  // Enable auto-commit for push model
    AutoCommitIntervalMs = 5000,  // Commit every 5 seconds
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
        var consumeResult = consumer.Consume();
        if (consumeResult is { Message.Value: var value })
        {
            Console.WriteLine($"Push Consumer: {JsonSerializer.Serialize(value)}");
            consumer.Commit(consumeResult);
        }
    }
    catch (ConsumeException e)
    {
        Console.WriteLine($"Push Consumer error: {e.Error.Reason}");
    }
    catch (JsonException e)
    {
        Console.WriteLine($"Push Consumer deserialization error: {e.Message}");
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
