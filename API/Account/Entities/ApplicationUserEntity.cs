using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using MultiTenantOpenProject.API.Tenancy.Entities;

namespace MultiTenantOpenProject.API.Account.Entities;

public class ApplicationUserEntity : IdentityUser<Guid>
{
    [ForeignKey(nameof(Tenant))]
    public Guid TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }
    public string? RefreshToken { get; set; }
    public string? LoginToken { get; set; }
    public DateTimeOffset? LoginTokenCreatedAt { get; set; } = null;
    public bool IsActive { get; set; } = true;

    public ApplicationUserEntity(
        Guid tenantId,
        string email) : base(email)
    {
        TenantId = tenantId;
        Email = email;
    }
}
