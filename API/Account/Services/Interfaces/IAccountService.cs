using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MultiTenantOpenProject.API.Account.Entities;
using MultiTenantOpenProject.API.Account.Models;

namespace MultiTenantOpenProject.API.Account.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ApplicationUserModel> RegisterAsync(ApplicationUserModel model);

        Task<TokenModel> LoginAsync(Guid userId, string logintoken);

        Task SignOutAsync(Guid userId);

        Task<(string token, Guid userId)> GenerateLoginTokenAsync(string userEmail);

        Task<ApplicationUserEntity?> FindByEmailAsync(string email);

        Task<ApplicationUserEntity> GetUserByIdAsync(Guid id);

        Task<TokenModel> GenerateTokenForUser(Guid userId);

        Task<IdentityResult> ConfirmEmailAsync(Guid userId, string code);

        Task ChangeAccountActivityStatusAsync(Guid userId, bool isActive);
    }
}
