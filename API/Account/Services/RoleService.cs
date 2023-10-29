using Microsoft.AspNetCore.Identity;
using MultiTenantOpenProject.API.Account.Entities;
using MultiTenantOpenProject.API.Account.Services.Interfaces;

namespace MultiTenantOpenProject.API.Account.Services;
public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRoleEntity> _roleManager;

    public RoleService(RoleManager<ApplicationRoleEntity> roleManager)
    {
        _roleManager = roleManager;
    }

    public void AddDefaultSystemRolesAsync()
    {
        // _roleManager.CreateAsync()
    }
}