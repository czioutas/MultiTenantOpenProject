using MultiTenantOpenProject.API.Application.Services.Interfaces;

namespace MultiTenantOpenProject.API.Services;

public class RequestTenant : IRequestTenant
{
    public Guid TenantId { get; private set; }

    public RequestTenant(
        ILogger<RequestTenant> logger
    )
    {
    }

    void IRequestTenant.SetTenantId(Guid tenantId)
    {
        TenantId = tenantId;
    }
}
