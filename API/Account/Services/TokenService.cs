using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MultiTenantOpenProject.API.Account.Entities;
using MultiTenantOpenProject.API.Account.Models;
using MultiTenantOpenProject.API.Account.Services.Interfaces;
using MultiTenantOpenProject.API.Application;
using MultiTenantOpenProject.API.Settings;

namespace MultiTenantOpenProject.API.Account.Services;
public class TokenService : ITokenService
{
    private readonly TokenProviderSettings _settings;
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly RoleManager<ApplicationRoleEntity> _roleManager;

    public TokenService(
        TokenProviderSettings settings,
        UserManager<ApplicationUserEntity> userManager,
        RoleManager<ApplicationRoleEntity> roleManager)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(TokenProviderSettings));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(UserManager<ApplicationUserEntity>));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(RoleManager<ApplicationRoleEntity>));
    }

    public async Task<TokenModel> GenerateTokenAsync(ApplicationUserEntity user)
    {
        var nowDt = DateTimeOffset.UtcNow;
        var nowDto = DateTimeOffset.UtcNow;

        return new TokenModel(
            Guid.Empty,
            await CreateTokenAsync(user, nowDt, nowDto, false),
            await CreateTokenAsync(user, nowDt, nowDto, true)
        );
    }

    public TokenModel RefreshToken(string longLivedToken)
    {
        var nowDt = DateTime.UtcNow;
        var nowDto = DateTimeOffset.UtcNow;

        return new TokenModel(
            Guid.Empty,
            CreateTokenFromLLT(new JwtSecurityToken(longLivedToken), nowDt, nowDto),
            longLivedToken
        );
    }

    public string CreateTokenFromLLT(JwtSecurityToken longLivedToken, DateTimeOffset nowDt, DateTimeOffset nowDtO)
    {
        TimeSpan _expiration = _settings.ShortExpiration;

        // Create the JWT and write it to a string
        JwtSecurityToken jwt = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: longLivedToken.Claims,
            notBefore: nowDt.DateTime,
            expires: nowDt.DateTime.Add(_expiration),
            signingCredentials: _settings.SigningCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    // this is not good enough
    // we must do it like dis https://stackoverflow.com/questions/42036810/asp-net-core-jwt-mapping-role-claims-to-claimsidentity/42037615#42037615
    public async Task<string> CreateTokenAsync(ApplicationUserEntity user, DateTimeOffset nowDt,
        DateTimeOffset nowDtO, bool longLived)
    {
        var claims = await GetValidClaimsAsync(user, nowDtO);

        TimeSpan _expiration = longLived ? _settings.LongExpiration : _settings.ShortExpiration;

        // Create the JWT and write it to a string
        JwtSecurityToken jwt = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: nowDt.DateTime,
            expires: nowDt.DateTime.Add(_expiration),
            signingCredentials: _settings.SigningCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public async Task<List<Claim>> GetValidClaimsAsync(ApplicationUserEntity user, DateTimeOffset nowDtO)
    {
        var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, nowDtO.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),
                new Claim(ClaimTypeConstants.TenantIdentifier, user.TenantId.ToString())
            };

        IList<string> roles = await _userManager.GetRolesAsync(user);

        foreach (string r in roles)
        {
            var role = await _roleManager.FindByNameAsync(r);

            if (role == null)
            {
                throw new Exception($"Role ( {r} ) is unknown");
            }

            var roleClaims = await _roleManager.GetClaimsAsync(role);

            foreach (Claim roleClaim in roleClaims)
            {
                if (claims.FirstOrDefault(c => c.Type == roleClaim.Type) == null)
                {
                    claims.Add(roleClaim);
                }
            }
        }

        return claims;
    }
}