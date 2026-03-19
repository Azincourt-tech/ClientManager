namespace ClientManager.Domain.Core.Events;

public record CustomerCreatedEvent(
    Guid CustomerId,
    string Name,
    string Email,
    DateTime CreatedAt);
