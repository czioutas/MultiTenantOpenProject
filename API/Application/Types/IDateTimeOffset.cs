using System;

namespace MultiTenantOpenProject.API.Types
{
    public interface IDateTimeOffset
    {
        DateTimeOffset Now { get; }
    }
}
