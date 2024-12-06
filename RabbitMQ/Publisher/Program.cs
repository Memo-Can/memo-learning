using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

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
        var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync("queue-test", true, false, false);

        var messageBody = Encoding.UTF8.GetBytes("Test Message sent by publisher");

        await channel.BasicPublishAsync(string.Empty, "queue-test",false, messageBody);

        Console.WriteLine("The message send");
        Console.ReadLine();

    }
}