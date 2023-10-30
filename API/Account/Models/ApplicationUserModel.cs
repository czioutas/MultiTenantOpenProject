using MultiTenantOpenProject.API.Models;

namespace MultiTenantOpenProject.API.Account.Models;
public class ApplicationUserModel : BaseModel
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public ApplicationUserModel(
        string email,
        string password) : base()
    {
        Email = email;
        Password = password;
    }

    public ApplicationUserModel(
        Guid id,
        string email,
        string password) : base()
    {
        Id = id;
        Email = email;
        Password = password;
    }
}
