using System.Reflection.PortableExecutable;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Moq;
using MultiTenantOpenProject.API.Account.Entities;
using MultiTenantOpenProject.API.Application.Services.Interfaces;
using MultiTenantOpenProject.API.Data;
using MultiTenantOpenProject.API.Repositories;
using MultiTenantOpenProject.API.Tenancy.Entities;
using MultiTenantOpenProject.API.Types;

internal class MockITenancyRepository
{
    public static List<TenantEntity> GetTenantsFixture()
    {
        List<TenantEntity> tenants = new List<TenantEntity>()
        {
            new TenantEntity()
            {
                Id = new Guid("e5ca917f-4986-4424-bd6d-cdff676c111a"),
                Identifier = "defaultTenant1",
            },
            new TenantEntity()
            {
                Id = new Guid("d3ec49c8-47a8-4bab-ab08-24e53da45af4"),
                Identifier = "defaultTenant2",
            },
        };

        tenants[0].ApplicationUsers.Add(new ApplicationUserEntity(new Guid("492a2036-1cdb-438a-b4fc-d21e3f34b837"), tenants[0].Id, "user1_tenant1@gmail.com"));
        tenants[0].ApplicationUsers.Add(new ApplicationUserEntity(new Guid("e389103b-81b4-4cd0-ae77-fca8e6ed32e2"), tenants[0].Id, "user2_tenant1@gmail.com"));

        tenants[1].ApplicationUsers.Add(new ApplicationUserEntity(new Guid("90e2bf69-10c7-4360-826e-b5a3d96a3da0"), tenants[1].Id, "user1_tenant2@gmail.com"));
        tenants[1].ApplicationUsers.Add(new ApplicationUserEntity(new Guid("0baf6bf0-e2e1-4bf6-9863-42e9ace4e0f4"), tenants[1].Id, "user2_tenant2@gmail.com"));

        return tenants;
    }

    public static IRepository<TenantEntity> GetMock(Mock<IRequestTenant> mockIRequestTenant)
    {
        IDateTimeOffset dateTimeOff = new MachineDateTimeOffset();
        var mockITenantRepository = new Mock<IRepository<TenantEntity>>();

        var _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase("BloggingControllerTest")
        .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
        .Options;

        var context = new ApplicationDbContext(mockIRequestTenant.Object, _contextOptions, dateTimeOff);

        if (context.Tenants.Count() == 0)
        {
            context.Tenants.AddRange(GetTenantsFixture());
            context.SaveChanges();
        }

        IRepository<TenantEntity> t = new Repository<TenantEntity>(context);

        return t;
    }
}
