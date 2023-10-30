namespace MultiTenantOpenProject.Contracts.Account;

public sealed record TokenContract
{
    public string ShortToken { get; set; }
    public string LongToken { get; set; }

    public TokenContract(string shortToken, string longToken)
    {
        ShortToken = shortToken;
        LongToken = longToken;
    }
}

