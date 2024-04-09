using System.Text;
using Order.Models;
using Order.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Order.Services;

public class MessageService : IMessageService
{
    private readonly IModel _channel;

    public MessageService()
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
        _channel.QueueDeclare(queue: "orderqueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void MessageOrder(UserOrder userOrder)
    {
        var message = $"{userOrder.Name};{userOrder.Email};{userOrder.Product}";

        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "",
            routingKey: "orderqueue",
            basicProperties: null,
            body: body);
    }

    private void OrderConfirmation()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
        };

        _channel.BasicConsume(queue: "orderresponse",
            autoAck: true,
            consumer: consumer);
    }
}