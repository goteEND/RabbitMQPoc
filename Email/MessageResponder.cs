using System.Text;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Email;

public class MessageResponder
{
    private readonly IModel _channel;
    
    public MessageResponder()
    {
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "orderresponse",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }
    
    public void RespondMessage(byte[] body)
    {
        var response = BeautifyMessage(body);
        _channel.BasicPublish(exchange: "",
            routingKey: "orderresponse",
            basicProperties: null,
            body: response);
    }
    
    private static byte[] BeautifyMessage(byte[] body)
    {
        var message = Encoding.UTF8.GetString(body);
        
        Console.WriteLine($"BeautifyMessage: {message}");
        
        var splitMessage = message.Split(';');
        
        var response = $"Order for {splitMessage[0]} consisting {splitMessage[2]} sent to {splitMessage[1]}";
        
        return Encoding.UTF8.GetBytes(response);
    }
}