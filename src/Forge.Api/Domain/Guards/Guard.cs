using Forge.Api.Domain.Exceptions;

namespace Forge.Api.Domain.Guards;

internal static class Guard
{
    public static string Required(string value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{parameterName} is required.");

        value = value.Trim();

        if (value.Length > maxLength)
            throw new DomainException($"{parameterName} must be at most {maxLength} characters.");

        return value;
    }

    public static void NotNull(object? value, string parameterName)
    {
        if (value is null)
            throw new DomainException($"{parameterName} is required.");
    }

    public static void Positive(decimal value, string parameterName)
    {
        if (value <= 0)
            throw new DomainException($"{parameterName} must be a positive number.");
    }

    public static void NotDefault<T>(T value, string parameterName) where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
            throw new DomainException($"{parameterName} must not be the default value.");
    }

    public static string? Optional(string? value, string parameterName, int maxLength)
    {
        if (value is null) return null;

        value = value.Trim();
        if (value.Length == 0) return null;

        if (value.Length > maxLength)
            throw new DomainException($"{parameterName} must be at most {maxLength} characters.");

        return value;
    }

    public static T InRange<T>(T value, string parameterName, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new DomainException($"{parameterName} must be between {min} and {max}.");

        return value;
    }
}
