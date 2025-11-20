namespace TelegramConstructor.Domain.ValueObjects;

public class ApiCredentials
{
    public int ApiId { get; private set; }
    public string ApiHash { get; private set; }

    private ApiCredentials(int apiId, string apiHash)
    {
        ApiId = apiId;
        ApiHash = apiHash;
    }

    public static ApiCredentials Create(int apiId, string apiHash)
    {
        if (apiId <= 0)
            throw new ArgumentException("API ID must be positive", nameof(apiId));

        if (string.IsNullOrWhiteSpace(apiHash))
            throw new ArgumentException("API Hash cannot be empty", nameof(apiHash));

        if (apiHash.Length != 32)
            throw new ArgumentException("API Hash must be 32 characters", nameof(apiHash));

        return new ApiCredentials(apiId, apiHash);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ApiCredentials other)
            return false;

        return ApiId == other.ApiId && ApiHash == other.ApiHash;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ApiId, ApiHash);
    }
}
