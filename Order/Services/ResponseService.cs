using System.Text;
using Order.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Order.Services;

public class ResponseService : IResponseService
{
    private readonly IModel _channel;
    
    public ResponseService()
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

    public Task<string> OrderConfirmation()
    {
        var tcs = new TaskCompletionSource<string>();
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            tcs.SetResult(message);
        };

        _channel.BasicConsume(queue: "orderresponse",
            autoAck: true,
            consumer: consumer);

        return tcs.Task;
    }
}