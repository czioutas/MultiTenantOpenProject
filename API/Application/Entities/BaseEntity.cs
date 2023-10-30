namespace MultiTenantOpenProject.API.Entities
{
    public class BaseEntity
    {
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public BaseEntity()
        {
        }
    }
}
