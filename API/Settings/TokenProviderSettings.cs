using System;
using Microsoft.IdentityModel.Tokens;

namespace MultiTenantOpenProject.API.Settings
{
    public class TokenProviderSettings
    {
        public string Path { get; set; } = "/account/login";
        public string Issuer { get; set; } = "IssuerMultiTenantOpenProject";
        public string Audience { get; set; } = "AudienceMultiTenantOpenProject";
        public TimeSpan ShortExpiration { get; set; } = TimeSpan.FromDays(1);
        public TimeSpan LongExpiration { get; set; } = TimeSpan.FromDays(10);
        public string SecretKey { get; set; } = String.Empty;
        public SigningCredentials? SigningCredentials { get; set; }
    }
}
