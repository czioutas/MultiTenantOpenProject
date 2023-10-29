using Mapster;
using Microsoft.AspNetCore.Identity;
using MultiTenantOpenProject.API.Account.Entities;
using MultiTenantOpenProject.API.Account.Models;
using MultiTenantOpenProject.API.Account.Services.Interfaces;
using MultiTenantOpenProject.API.Application.Services.Interfaces;
using MultiTenantOpenProject.API.Exceptions;
using MultiTenantOpenProject.API.Types;

namespace MultiTenantOpenProject.API.Account.Services;
public class AccountService : IAccountService
{
    private readonly IRequestTenant _requestTenant;
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly SignInManager<ApplicationUserEntity> _signInManager;
    // private readonly IMessageService _messageService;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeOffset _dateTime;

    public AccountService(
        IRequestTenant requestTenant,
        UserManager<ApplicationUserEntity> userManager,
        SignInManager<ApplicationUserEntity> signInManager,
        ITokenService tokenService,
        IDateTimeOffset dateTime)
    {
        _requestTenant = requestTenant ?? throw new ArgumentNullException(nameof(requestTenant));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        // _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
    }

    public async Task<ApplicationUserModel> RegisterAsync(ApplicationUserModel model)
    {
        var userEntity = new ApplicationUserEntity(tenantId: _requestTenant.TenantId, email: model.Email);
        var result = await _userManager.CreateAsync(userEntity, model.Password);

        if (!result.Succeeded)
        {
            throw new IdentityException(result.Errors);
        }

        return userEntity.Adapt<ApplicationUserModel>();
    }

    public async Task<ApplicationUserEntity?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUserEntity> GetUserByIdAsync(Guid id)
    {
        ApplicationUserEntity? userEntity = await _userManager.FindByIdAsync(id.ToString());

        if (userEntity is null)
        {
            throw new ResourceNotFoundException();
        }

        return userEntity;
    }

    public async Task SignOutAsync(Guid userId)
    {
        ApplicationUserEntity userEntity = await GetUserByIdAsync(userId);

        userEntity.RefreshToken = string.Empty;
        await _userManager.UpdateAsync(userEntity);
    }

    public async Task<TokenModel> GenerateTokenForUser(Guid userId)
    {
        ApplicationUserEntity user = await GetUserByIdAsync(userId);
        TokenModel tokens = await _tokenService.GenerateTokenAsync(user);

        user.RefreshToken = tokens.LongToken;
        await _userManager.UpdateAsync(user);

        return tokens;
    }

    public async Task<IdentityResult> ConfirmEmailAsync(Guid userId, string code)
    {
        var user = await GetUserByIdAsync(userId);
        return await _userManager.ConfirmEmailAsync(user, code);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<IdentityResult> ResetPasswordAsync(Guid userId, string token, string password)
    {
        var user = await GetUserByIdAsync(userId);
        return await _userManager.ResetPasswordAsync(user, token, password);
    }

    public async Task<(string token, Guid userId)> GenerateLoginTokenAsync(string userEmail)
    {
        ApplicationUserEntity? userEntity = await _userManager.FindByEmailAsync(userEmail);

        if (userEntity is null)
        {
            throw new ResourceNotFoundException();
        }

        string loginToken = Guid.NewGuid().ToString("N");
        DateTimeOffset loginTokenCreatedAt = _dateTime.Now.ToUniversalTime();

        userEntity.LoginToken = loginToken;
        userEntity.LoginTokenCreatedAt = loginTokenCreatedAt;

        await _userManager.UpdateAsync(userEntity);

        return (loginToken, userEntity.Id);
    }

    /// <summary>
    /// Checks
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="logintoken"></param>
    /// <returns></returns>
    public async Task<TokenModel> LoginAsync(Guid userId, string logintoken)
    {
        ApplicationUserEntity user = await GetUserByIdAsync(userId);
        DateTime currentTime = _dateTime.Now.ToUniversalTime().DateTime;

        if (
            user.LoginToken != logintoken ||
            user.LoginTokenCreatedAt.HasValue == false ||
            currentTime > user.LoginTokenCreatedAt.Value.AddMinutes(5).ToUniversalTime().DateTime
        )
        {
            throw new ResourceNotFoundException();
        }

        user.LoginToken = string.Empty;
        user.LoginTokenCreatedAt = null;

        return await GenerateTokenForUser(userId);
    }

    public async Task ChangeAccountActivityStatusAsync(Guid userId, bool isActive)
    {
        ApplicationUserEntity user = await GetUserByIdAsync(userId);
        user.IsActive = isActive;
        await _userManager.UpdateAsync(user);
    }
}
