namespace MultiTenantOpenProject.API.Application.Settings;

public class RedisSettings
{
    /// <summary>
    /// The IPAddress to connect to.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// The port to connect to.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// The password for the Redis connection
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Value that allows admin functionality via the Redis connection.
    /// Defaults to false.
    /// </summary>
    public bool AllowAdmin { get; set; } = false;

    /// <summary>
    /// The name of the client that wishes or is connected
    /// </summary>
    public string ClientName { get; set; } = string.Empty;

    /// <summary>
    /// The version of Redis we are targeting. Defaults to "5.0.7"
    /// </summary>
    public string RedisVersion { get; set; } = "5.0.7";
}
