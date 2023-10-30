namespace MultiTenantOpenProject.Contracts.Account;
public sealed record UserContract
{
    public string Name { get; init; } = string.Empty;
    public int Age { get; init; }

    public Guid[] ImageIds { get; init; }
}
