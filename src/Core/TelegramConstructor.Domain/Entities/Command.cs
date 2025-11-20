using TelegramConstructor.Domain.Common;

namespace TelegramConstructor.Domain.Entities;

public class Command : BaseEntity
{
    public Guid BotId { get; private set; }
    public string Pattern { get; private set; }
    public string? Description { get; private set; }
    public string? HandlerData { get; private set; } // JSON data
    public int Order { get; private set; }
    public bool IsEnabled { get; private set; }

    // Navigation property
    public Bot Bot { get; private set; } = null!;

    private Command() { } // For EF Core

    public Command(Guid botId, string pattern, string? description = null)
    {
        if (botId == Guid.Empty)
            throw new ArgumentException("Bot ID cannot be empty", nameof(botId));

        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentException("Command pattern cannot be empty", nameof(pattern));

        BotId = botId;
        Pattern = pattern;
        Description = description;
        IsEnabled = true;
        Order = 0;
    }

    public void UpdatePattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentException("Command pattern cannot be empty", nameof(pattern));

        Pattern = pattern;
        UpdateTimestamp();
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdateTimestamp();
    }

    public void UpdateHandlerData(string handlerData)
    {
        HandlerData = handlerData;
        UpdateTimestamp();
    }

    public void UpdateOrder(int order)
    {
        if (order < 0)
            throw new ArgumentException("Order cannot be negative", nameof(order));

        Order = order;
        UpdateTimestamp();
    }

    public void Enable()
    {
        IsEnabled = true;
        UpdateTimestamp();
    }

    public void Disable()
    {
        IsEnabled = false;
        UpdateTimestamp();
    }
}
