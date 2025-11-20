using TelegramConstructor.Domain.Common;
using TelegramConstructor.Domain.Enums;
using TelegramConstructor.Domain.ValueObjects;

namespace TelegramConstructor.Domain.Entities;

public class Bot : BaseEntity
{
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; }
    public BotType Type { get; private set; }
    public BotStatus Status { get; private set; }
    public string? TokenValue { get; private set; } // Stored encrypted
    public string? Configuration { get; private set; } // JSON configuration

    private readonly List<Command> _commands = new();
    public IReadOnlyCollection<Command> Commands => _commands.AsReadOnly();

    // Navigation property
    public Project Project { get; private set; } = null!;

    private Bot() { } // For EF Core

    public Bot(Guid projectId, string name, BotType type)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("Project ID cannot be empty", nameof(projectId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Bot name cannot be empty", nameof(name));

        ProjectId = projectId;
        Name = name;
        Type = type;
        Status = BotStatus.Draft;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Bot name cannot be empty", nameof(name));

        Name = name;
        UpdateTimestamp();
    }

    public void UpdateStatus(BotStatus status)
    {
        Status = status;
        UpdateTimestamp();
    }

    public void UpdateToken(string tokenValue)
    {
        if (string.IsNullOrWhiteSpace(tokenValue))
            throw new ArgumentException("Token cannot be empty", nameof(tokenValue));

        TokenValue = tokenValue;
        UpdateTimestamp();
    }

    public void UpdateConfiguration(string configuration)
    {
        Configuration = configuration;
        UpdateTimestamp();
    }

    public void AddCommand(Command command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        _commands.Add(command);
        UpdateTimestamp();
    }

    public void RemoveCommand(Command command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        _commands.Remove(command);
        UpdateTimestamp();
    }
}
