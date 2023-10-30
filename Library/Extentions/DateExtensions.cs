namespace MultiTenantOpenProject.Library.Extensions;

/// <summary>
/// Extensions methods for Enums
/// </summary>
public static class DateExtensions
{
    public static int GetAge(this DateOnly value)
    {
        int age = 0;
        age = DateTime.UtcNow.Year - value.Year;
        if (DateTime.UtcNow.DayOfYear < value.DayOfYear)
            age = age - 1;

        return age;
    }

    public static int GetAge(this DateTime value)
    {
        int age = 0;
        age = DateTime.UtcNow.Year - value.Year;
        if (DateTime.UtcNow.DayOfYear < value.DayOfYear)
            age = age - 1;

        return age;
    }

    public static int GetAge(this DateTimeOffset value)
    {
        int age = 0;
        age = DateTime.UtcNow.Year - value.UtcDateTime.Year;
        if (DateTime.UtcNow.DayOfYear < value.UtcDateTime.DayOfYear)
            age = age - 1;

        return age;
    }
}
