using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MultiTenantOpenProject.API.Application.Services.Interfaces;
using MultiTenantOpenProject.API.Exceptions;
using MultiTenantOpenProject.API.Repositories;
using MultiTenantOpenProject.API.Tenancy.Entities;
using MultiTenantOpenProject.API.Tenancy.Services;
using MultiTenantOpenProject.API.Tenancy.Services.Interfaces;

namespace API.Tests;

[TestClass]
public class TenancyServiceTests
{
    private ITenantService _tenantService;
    private Mock<IRequestTenant> _mockIRequestTenant;
    private IRepository<TenantEntity> _tenantRepository;

    List<TenantEntity> tenantsFixture = MockITenancyRepository.GetTenantsFixture();

    public TenancyServiceTests()
    {
        _mockIRequestTenant = new Mock<IRequestTenant>();
        _tenantRepository = MockITenancyRepository.GetMock(_mockIRequestTenant);
        var mockLogger = new Mock<ILogger<TenantService>>();

        _tenantService = new TenantService(_tenantRepository, mockLogger.Object);
    }

    [TestMethod]
    public async Task Get_Tenant_Should_Return_For_Existing_Tenant_And_MemberAsync()
    {
        var tenantModel = await _tenantService.GetAsync(tenantsFixture[0].Id, tenantsFixture[0].ApplicationUsers.First().Id);

        Assert.IsNotNull(tenantModel);
    }

    [TestMethod]
    public async Task Get_Tenant_Should_Throw_For_Existing_Tenant_And_Non_MemberAsync()
    {
        await Assert.ThrowsExceptionAsync<ResourceNotFoundException>(async () =>
        {
            await _tenantService.GetAsync(tenantsFixture[0].Id, Guid.NewGuid());
        });
    }

    [TestMethod]
    public async Task Get_Tenant_Should_Throw_For_Non_Existent_Tenant()
    {
        await Assert.ThrowsExceptionAsync<ResourceNotFoundException>(async () =>
        {
            // We set a random Guid for the Tenant Id, UserId is irrelevant
            await _tenantService.GetAsync(Guid.NewGuid(), tenantsFixture[0].ApplicationUsers.First().Id);
        });
    }
}
