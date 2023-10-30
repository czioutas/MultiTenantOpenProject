namespace MultiTenantOpenProject.Contracts.Tenancy;

public sealed record TenantContract
{
    public string Identifier { get; set; }

    public Guid Id { get; set; }

    public TenantContract(string identifier, Guid id)
    {
        Identifier = identifier;
        Id = id;
    }
}
