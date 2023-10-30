using MultiTenantOpenProject.API.Tenancy.Models;

namespace MultiTenantOpenProject.API.Tenancy.Services.Interfaces;

public interface ITenantService
{
    Task<TenantModel> CreateAsync(string tenantIdentifier);
    Task<TenantModel> FindByIdentifierAsync(string tenantIdentifier);
    Task<TenantModel> GetAsync(Guid id, Guid UserId);
}
