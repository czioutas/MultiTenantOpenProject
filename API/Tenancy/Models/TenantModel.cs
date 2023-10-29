using MultiTenantOpenProject.API.Models;

namespace MultiTenantOpenProject.API.Tenancy.Models;

public class TenantModel : BaseModel
{
    public Guid Id { get; set; }
    public string Identifier { get; set; }

    public TenantModel(Guid id, string identifier) : base()
    {
        Id = id;
        Identifier = identifier;
    }
}