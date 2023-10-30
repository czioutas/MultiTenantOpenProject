namespace MultiTenantOpenProject.ContractsAccount;

public sealed record LoginContract
{
    public string Email { get; set; }

    public string Password { get; set; }

    public LoginContract(string email, string password)
    {
        Email = email;
        Password = password;
    }
}
