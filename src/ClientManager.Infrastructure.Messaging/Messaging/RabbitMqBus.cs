using System.Text;
using System.Text.Json;
using ClientManager.Domain.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ClientManager.Infrastructure.Messaging.Messaging;

public class RabbitMqBus : IMessageBus, IDisposable
{
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqBus(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private async Task EnsureConnectedAsync()
    {
        if (_connection != null && _connection.IsOpen && _channel != null && _channel.IsOpen)
        {
            return;
        }

        var factory = new ConnectionFactory
        {
            Uri = new Uri(_configuration.GetConnectionString("RabbitMQ") ?? throw new InvalidOperationException("RabbitMQ connection string not found."))
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
    }

    public async Task PublishAsync<T>(T message, string queueName)
    {
        await EnsureConnectedAsync();

        await _channel!.QueueDeclareAsync(queue: queueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        await _channel.BasicPublishAsync(exchange: string.Empty,
                             routingKey: queueName,
                             body: body);
    }

    public async Task SubscribeAsync<T>(string queueName, Func<T, Task> onMessage)
    {
        await EnsureConnectedAsync();

        await _channel!.QueueDeclareAsync(queue: queueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));

            if (message != null)
            {
                await onMessage(message);
            }
            
            // Ack manually if using manual ack, but for now we'll rely on default
            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
