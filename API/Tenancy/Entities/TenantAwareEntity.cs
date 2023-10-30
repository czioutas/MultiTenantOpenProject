using System.ComponentModel.DataAnnotations.Schema;
using MultiTenantOpenProject.API.Entities;

namespace MultiTenantOpenProject.API.Tenancy.Entities;

public class TenantAwareEntity : BaseEntity
{
    [ForeignKey(nameof(Tenant))]
    public Guid TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }
}
