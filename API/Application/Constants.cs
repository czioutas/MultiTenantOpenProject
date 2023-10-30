namespace MultiTenantOpenProject.API.Application;

public static class Constants
{
    public const string DE = "de";
}

public static class Roles
{
    /// <summary>Name of the Role for a Admin User</summary>
    public const string AdminUserRole = "adminUserRole";

    /// <summary>Name of the Role for a Simple User</summary>
    public const string SimpleUserRole = "simpleUserRole";
}

public enum UserType
{
    Admin = 1,
    Simple = 2
}

public static class ClaimTypeConstants
{
    public const string Email = "Email";
    public const string UserName = "Username";
    public const string EmailVerified = "EmailVerified";
    public const string UserId = "UserId";
    public const string TenantIdentifier = "TenantIdentifier";
}
