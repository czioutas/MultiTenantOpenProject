using System;

namespace MultiTenantOpenProject.API.Types
{
    public class MachineDateTimeOffset : IDateTimeOffset
    {
        public DateTimeOffset Now => DateTimeOffset.Now;

        public static int CurrentYear => DateTimeOffset.Now.Year;
    }
}
