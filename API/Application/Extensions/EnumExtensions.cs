namespace MultiTenantOpenProject.API.Extensions;

/// <summary>
/// Extensions methods for Enums
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Returns the name of the enum value as a string
    /// </summary>
    /// <param name="value"></param>
    /// <returns>The name of the enum value</returns>
    public static string GetName(this Enum value)
    {
        return Enum.GetName(value.GetType(), value);
    }

    /// <summary>
    /// Convenience method. Returns true if "value" parameter is on the "oneOfValues" list.
    /// </summary>
    public static bool IsOneOf<T>(this T value, params T[] oneOfValues) where T : Enum
    {
        return oneOfValues.Any(x => x.Equals(value));
    }
}