using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Email;

public class Program
{
    private static readonly ConnectionFactory Factory = new()
    {
        HostName = "localhost",
        Port = 5672,
        UserName = "guest",
        Password = "guest"
    };
    private static readonly IConnection Connection = Factory.CreateConnection();
    private static readonly IModel Channel = Connection.CreateModel();
    
    public static void Main()
    {
        Channel.QueueDeclare(queue: "orderqueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(Channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);
            RespondMessage(body);

        };
        
        Channel.BasicConsume(queue: "orderqueue",
            autoAck: true,
            consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private static void RespondMessage(byte[] body)
    {
        Channel.BasicPublish(exchange: "",
            routingKey: "orderqueue",
            basicProperties: null,
            body: body);
    }
    
}

