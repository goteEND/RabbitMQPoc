using System.Text;
using Order.Models;
using Order.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Order.Services;

public class MessageService : IMessageService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageService()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "orderqueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public string MessageOrder(UserOrder userOrder)
    {
        var message = $"{userOrder.Name};{userOrder.Email};{userOrder.Product}";

        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "",
            routingKey: "orderqueue",
            basicProperties: null,
            body: body);

        var response = OrderConfirmation();
        return response;
    }

    private string OrderConfirmation()
    {
        var tcs = new TaskCompletionSource<string>();
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            tcs.SetResult(message);
        };

        _channel.BasicConsume(queue: "orderqueue",
            autoAck: true,
            consumer: consumer);

        return tcs.Task.Result;
    }

    public void Close()
    {
        _channel.Close();
        _connection.Close();
    }

}