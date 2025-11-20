namespace TelegramConstructor.Domain.ValueObjects;

public class BotToken
{
    public string Value { get; private set; }

    private BotToken(string value)
    {
        Value = value;
    }

    public static BotToken Create(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Bot token cannot be empty", nameof(token));

        if (!IsValidTokenFormat(token))
            throw new ArgumentException("Invalid bot token format", nameof(token));

        return new BotToken(token);
    }

    private static bool IsValidTokenFormat(string token)
    {
        // Basic validation: token should contain ':' and be at least 45 characters
        return token.Contains(':') && token.Length >= 45;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not BotToken other)
            return false;

        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        // Return masked token for security
        var parts = Value.Split(':');
        return parts.Length == 2 ? $"{parts[0]}:***" : "***";
    }
}
