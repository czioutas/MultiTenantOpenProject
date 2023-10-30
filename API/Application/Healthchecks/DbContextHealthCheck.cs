using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MultiTenantOpenProject.API.Data;

namespace MultiTenantOpenProject.API.Application.HealthChecks
{
    public class DbContextHealthCheck : IHealthCheck
    {
        private readonly ApplicationDbContext _dbContext;

        public DbContextHealthCheck(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _dbContext.Database.CanConnectAsync(cancellationToken) ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
    }
}
