
namespace MultiTenantOpenProject.Contracts.Account;
/// <summary>
/// Contract used to transmit required data for registration
/// </summary>
public sealed record RegisterContract
{
    /// <summary>
    /// The Email of the User
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// The Email of the User
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Constructor used to create the RegisterContract
    /// </summary>
    /// <param name="email">The email of the User</param>
    /// <param name="password">The password of the User</param>
    public RegisterContract(
        string password,
        string email)
    {
        Password = password;
        Email = email;
    }
}
