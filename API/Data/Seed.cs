using Bogus;
using MultiTenantOpenProject.API.Tenancy.Models;
using MultiTenantOpenProject.API.Tenancy.Services.Interfaces;

namespace MultiTenantOpenProject.API.Data;

public class Seed
{
    public static TenantModel? defaultTenant1;
    public static TenantModel? defaultTenant2;


    public static async Task SeedAsync(IServiceProvider services)
    {

    }

    public static async Task DevelopmentSeedAsync(IServiceProvider services)
    {
        await AddDefaultTenantAsync(services);
    }

    private static async Task AddDefaultTenantAsync(IServiceProvider services)
    {
        ITenantService tenantService = services.GetRequiredService<ITenantService>();

        defaultTenant1 = await tenantService.CreateAsync("defaultTenant1");
        defaultTenant2 = await tenantService.CreateAsync("defaultTenant2");
    }
}
