using System.Security.Claims;
using MultiTenantOpenProject.API.Account.Entities;
using MultiTenantOpenProject.API.Account.Models;

namespace MultiTenantOpenProject.API.Account.Services.Interfaces;

public interface ITokenService
{
    Task<TokenModel> GenerateTokenAsync(ApplicationUserEntity user);

    TokenModel RefreshToken(string longLivedToken);

    Task<string> CreateTokenAsync(
        ApplicationUserEntity user,
        DateTimeOffset nowDt,
        DateTimeOffset nowDtO,
        bool longLived
    );

    Task<List<Claim>> GetValidClaimsAsync(ApplicationUserEntity user, DateTimeOffset nowDtO);
}
