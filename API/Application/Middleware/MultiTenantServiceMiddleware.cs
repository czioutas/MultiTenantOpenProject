using MultiTenantOpenProject.API.Tenancy.Services.Interfaces;

namespace MultiTenantOpenProject.API.Application.Middleware;

public class MultiTenantServiceMiddleware : IMiddleware
{
    private readonly ITenantService _tenantService;
    public MultiTenantServiceMiddleware(ITenantService tenantService)
    {
        _tenantService = tenantService;
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
            _tenantService.SetTenantId(parsedTenantId);
        }

        await next(context);
    }
}
