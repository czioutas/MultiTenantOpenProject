namespace MultiTenantOpenProject.Contracts;
public sealed record AuthenticatedUserContract
{
    public string Name { get; init; } = string.Empty;
    public int Age { get; init; }

    public Guid[]? ImageIds { get; init; }
}
