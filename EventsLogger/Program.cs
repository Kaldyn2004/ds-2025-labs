using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory {
HostName = "localhost",
UserName = "kirill",
Password = "12345",
};
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "logs",
    type: ExchangeType.Fanout);

// declare a server-named queue
QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
string queueName = queueDeclareResult.QueueName;
await channel.QueueBindAsync(queue: queueName, exchange: "logs", routingKey: string.Empty);

Console.WriteLine(" [*] Waiting for logs.");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    try
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        // Парсим JSON или обрабатываем как plain text, если это не JSON
        try
        {
            var jsonDocument = JsonDocument.Parse(message);
            var formattedJson = JsonSerializer.Serialize(
                jsonDocument,
                new JsonSerializerOptions { WriteIndented = true });

            Console.WriteLine($" [x] Received JSON:\n{formattedJson}");
        }
        catch (JsonException)
        {
            Console.WriteLine($" [x] Received plain message: {message}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($" [!] Error processing message: {ex.Message}");
    }
};

await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();