using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using MultiTenantOpenProject.API.Tenancy.Entities;

namespace MultiTenantOpenProject.API.Account.Entities;

public partial class ApplicationRoleEntity : IdentityRole<Guid>
{
    [ForeignKey(nameof(Tenant))]
    public Guid TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }

    public ApplicationRoleEntity() { }

    public ApplicationRoleEntity(
        string roleName
    ) : base(roleName)
    {
    }

    public ApplicationRoleEntity(
        Guid tenantId,
        string roleName
    ) : base(roleName)
    {
        TenantId = tenantId;
    }
}
