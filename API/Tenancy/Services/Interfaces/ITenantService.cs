using MultiTenantOpenProject.API.Tenancy.Models;

namespace MultiTenantOpenProject.API.Tenancy.Services.Interfaces;

public interface ITenantService
{
    Guid TenantId { get; }
    Task<TenantModel> CreateAsync(string tenantIdentifier);
    Task<TenantModel> FindByIdentifierAsync(string tenantIdentifier);
    void SetTenantId(Guid tenantId);
}