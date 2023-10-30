using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MultiTenantOpenProject.API.Account.Entities;
using MultiTenantOpenProject.API.Entities;

namespace MultiTenantOpenProject.API.Tenancy.Entities;

public class TenantEntity : BaseEntity
{
    [Key, Column("Id")]
    public Guid Id { get; set; }

    public string Identifier { get; set; } // the identifier can change but not the Id

    public ICollection<ApplicationUserEntity> ApplicationUsers { get; set; } = new List<ApplicationUserEntity>(); // Collection navigation containing dependents

    public TenantEntity()
    {
        Identifier = string.Empty;
    }

    public TenantEntity(string identifier)
    {
        Identifier = identifier;
    }
}
