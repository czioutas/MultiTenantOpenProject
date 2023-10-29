using System.ComponentModel.DataAnnotations.Schema;
using MultiTenantOpenProject.API.Entities;

namespace MultiTenantOpenProject.API.Account.Entities;

public class UserProfileEntity : BaseEntity
{
    [ForeignKey(nameof(ApplicationUserEntity))]
    public Guid ApplicationUserId { get; set; }
    public ApplicationUserEntity? ApplicationUser { get; set; }

    public string? SomeValue { get; set; }

    public UserProfileEntity()
    {
    }

    public UserProfileEntity(Guid id, string identifier)
    {
    }
}