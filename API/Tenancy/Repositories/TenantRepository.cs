using MultiTenantOpenProject.API.Data;
using MultiTenantOpenProject.API.Repositories;
using MultiTenantOpenProject.API.Tenancy.Entities;

namespace MultiTenantOpenProject.API.Tenancy.Repositories;
public class TenantRepository : Repository<TenantEntity>
{
    public TenantRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
