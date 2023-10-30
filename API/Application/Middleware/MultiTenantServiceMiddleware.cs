using MultiTenantOpenProject.API.Application.Services.Interfaces;
using MultiTenantOpenProject.API.Tenancy.Services.Interfaces;

namespace MultiTenantOpenProject.API.Application.Middleware;

public class MultiTenantServiceMiddleware : IMiddleware
{
    private readonly IRequestTenant _requestTenant;
    public MultiTenantServiceMiddleware(IRequestTenant requestTenant)
    {
        _requestTenant = requestTenant;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string? tenantId;

        tenantId = context.Request.Headers["tenantId"];

        if (string.IsNullOrEmpty(tenantId) && context.Request.Query.TryGetValue("tenantId", out var values))
        {
            tenantId = values.FirstOrDefault();
        }


        if (!string.IsNullOrEmpty(tenantId) && Guid.TryParse(tenantId, out Guid parsedTenantId))
        {
            _requestTenant.SetTenantId(parsedTenantId);
        }

        await next(context);
    }
}
