using ClientManager.Domain.Core.Interfaces;
using ClientManager.Infrastructure.Messaging.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace ClientManager.Infrastructure.Messaging.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBus, RabbitMqBus>();
        return services;
    }
}
