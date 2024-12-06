using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory) 
                    .AddJsonFile("Settings.json", optional: true, reloadOnChange: true) 
                    .Build();


        var factory = new ConnectionFactory();

        factory.Uri = new Uri(configuration["AppSettings:Connection"]);

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        Console.WriteLine(" [*] Waiting for messages.");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");
            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync("queue-test", autoAck: true, consumer: consumer);

        Console.ReadLine();

    }
}