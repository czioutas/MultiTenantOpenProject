using Mapster;
using Microsoft.EntityFrameworkCore;
using MultiTenantOpenProject.API.Application.Services;
using MultiTenantOpenProject.API.Exceptions;
using MultiTenantOpenProject.API.Models;
using MultiTenantOpenProject.API.Repositories;
using MultiTenantOpenProject.API.Tenancy.Entities;
using MultiTenantOpenProject.API.Tenancy.Models;
using MultiTenantOpenProject.API.Tenancy.Services.Interfaces;

namespace MultiTenantOpenProject.API.Tenancy.Services;

public class TenantService : BaseService<TenantService>, ITenantService
{
    public readonly IRepository<TenantEntity> _repository;

    public TenantService(
        IRepository<TenantEntity> repository,
        ILogger<TenantService> logger
    ) : base(logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }


    public async Task<TenantModel> CreateAsync(string tenantIdentifier)
    {
        var _tenantEntity = await _repository.FirstByConditionAsync(t => t.Identifier == tenantIdentifier);
        if (_tenantEntity != null)
        {
            throw new ResourceAlreadyExistsException($"Tenant with Identifier {tenantIdentifier} could not be created.");
        }

        var tenantEntity = new TenantEntity() { Identifier = tenantIdentifier };
        await _repository.CreateAsync(tenantEntity);
        var result = await _repository.SaveAsync();

        if (result != 1)
        {
            throw new ServiceException(new ServiceError("Could not create Tenant " + tenantIdentifier));
        }
        else
        {
            return new TenantModel(tenantEntity.Id, tenantEntity.Identifier);
        }
    }

    public async Task<TenantModel> FindByIdentifierAsync(string tenantIdentifier)
    {
        var _tenantEntity = await _repository.FirstByConditionAsync(t => t.Identifier == tenantIdentifier);

        if (_tenantEntity != null)
        {
            return new TenantModel(_tenantEntity.Id, _tenantEntity.Identifier);
        }
        else
        {
            throw new ResourceNotFoundException();
        }
    }

    public async Task<TenantModel> GetAsync(Guid id, Guid userId)
    {
        TenantEntity? tenantEntity = await _repository.Context.Set<TenantEntity>()
        .Where(t => t.Id == id)
        .Include(t => t.ApplicationUsers)
        .Where(t => t.ApplicationUsers.FirstOrDefault(u => u.Id == userId) != null)
        .FirstOrDefaultAsync();

        if (tenantEntity is null)
        {
            throw new ResourceNotFoundException();
        }

        return tenantEntity.Adapt<TenantModel>();
    }

}
