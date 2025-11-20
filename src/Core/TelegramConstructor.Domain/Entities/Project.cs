using TelegramConstructor.Domain.Common;
using TelegramConstructor.Domain.Enums;

namespace TelegramConstructor.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public ProjectStatus Status { get; private set; }
    public string? Settings { get; private set; } // JSON settings

    private readonly List<Bot> _bots = new();
    public IReadOnlyCollection<Bot> Bots => _bots.AsReadOnly();

    private Project() { } // For EF Core

    public Project(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        Status = ProjectStatus.New;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name cannot be empty", nameof(name));

        Name = name;
        UpdateTimestamp();
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdateTimestamp();
    }

    public void UpdateStatus(ProjectStatus status)
    {
        Status = status;
        UpdateTimestamp();
    }

    public void UpdateSettings(string settings)
    {
        Settings = settings;
        UpdateTimestamp();
    }

    public void AddBot(Bot bot)
    {
        if (bot == null)
            throw new ArgumentNullException(nameof(bot));

        _bots.Add(bot);
        UpdateTimestamp();
    }

    public void RemoveBot(Bot bot)
    {
        if (bot == null)
            throw new ArgumentNullException(nameof(bot));

        _bots.Remove(bot);
        UpdateTimestamp();
    }
}
