using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using MultiTenantOpenProject.API.Account.Entities;
using MultiTenantOpenProject.API.Application.Services.Interfaces;
using MultiTenantOpenProject.API.Entities;
using MultiTenantOpenProject.API.Tenancy.Entities;
using MultiTenantOpenProject.API.Types;

namespace MultiTenantOpenProject.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUserEntity, ApplicationRoleEntity, Guid>
    {
        private readonly IRequestTenant _requestTenant;

        private readonly IDateTimeOffset _dateTime;

        public ApplicationDbContext(
            IRequestTenant requestTenant,
            DbContextOptions<ApplicationDbContext> options,
            IDateTimeOffset dateTime) :
            base(options)
        {
            _requestTenant = requestTenant ?? throw new ArgumentNullException(nameof(requestTenant));
            _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }

        // What is => Set<T>()? https://docs.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types#dbcontext-and-dbset

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ConfigurateTenant(builder);
            ConfigureRLSForEntities(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType).Property(nameof(BaseEntity.CreatedAt)).Metadata
                        .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                }
            }
        }

        private static void ConfigurateTenant(ModelBuilder builder)
        {
            builder.Entity<TenantEntity>().HasIndex(t => t.Identifier).IsUnique();
        }

        private void ConfigureRLSForEntities(ModelBuilder builder)
        {
            // builder.Entity<AnnouncementEntity>(entity =>
            // {
            //     entity.HasQueryFilter(e => e.TenantId == _tenant.TenantId);
            // });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            UpdateStatuses();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateStatuses();
            return base.SaveChanges();
        }

        private void DeleteDependent(EntityEntry entry)
        {
            entry.CurrentValues.SetValues(new { DeletedAt = _dateTime.Now, Deleted = true });
            entry.State = EntityState.Modified;
        }

        private void UpdateStatuses()
        {
            foreach (var entry in ChangeTracker.Entries<TenantAwareEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.TenantId = _requestTenant.TenantId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.TenantId = _requestTenant.TenantId;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = _dateTime.Now.UtcDateTime;
                        entry.Entity.UpdatedAt = null;
                        break;

                    case EntityState.Modified:
                        // can we update a deleted?
                        // entry.
                        entry.Entity.UpdatedAt = _dateTime.Now.UtcDateTime;
                        break;
                }
            }
        }
    }
}