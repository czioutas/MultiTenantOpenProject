using MultiTenantOpenProject.API.Models;

namespace MultiTenantOpenProject.API.Account.Models;

public class TokenModel : BaseModel
{
    public string ShortToken { get; set; }
    public string LongToken { get; set; }

    public TokenModel(Guid id, string shortToken, string longToken) : base()
    {
        ShortToken = shortToken;
        LongToken = longToken;
    }
}
