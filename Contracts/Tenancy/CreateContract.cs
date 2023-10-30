namespace MultiTenantOpenProject.Contracts.Tenancy;

public sealed record CreateContract
{
    public string Identifier { get; set; }

    public CreateContract(string identifier)
    {
        Identifier = identifier;
    }
}
