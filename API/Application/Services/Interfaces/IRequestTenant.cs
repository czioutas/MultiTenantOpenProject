namespace MultiTenantOpenProject.API.Application.Services.Interfaces;

public interface IRequestTenant
{
    Guid TenantId { get; }
    void SetTenantId(Guid tenantId);
}