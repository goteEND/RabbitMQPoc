using System.Text;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Email;

public class Receiver
{
    private readonly IModel _channel;
    
    public Receiver()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "orderqueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void ReceiveMessage()
    {
        _channel.QueueDeclare(queue: "orderqueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);
        };
        
        _channel.BasicConsume(queue: "orderqueue",
            autoAck: true,
            consumer: consumer);
    }
    
    private void RespondMessage(byte[] body)
    {
        _channel.BasicPublish(exchange: "",
            routingKey: "orderqueue",
            basicProperties: null,
            body: body);
    }
    
    private static string BeautifyMessage(string message)
    {
        Console.WriteLine($"BeautifyMessage: {message}");
        var splitMessage = message.Split(';');
        return $"Order for {splitMessage[0]} consisting {splitMessage[2]} sent to {splitMessage[1]}";
    }
}