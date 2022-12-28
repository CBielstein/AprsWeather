using AprsSharp.Parsers.Aprs;
using Geolocation;

namespace AprsWeatherClient.Extensions;

/// <summary>
/// Extenion methods to help compute display values.
/// </summary>
public static class DisplayExtensions
{
    /// <summary>
    /// Converts an int to string with a concatenated value.
    /// </summary>
    /// <param name="input">Int to convert</param>
    /// <param name="suffix">Suffix to concatentate</param>
    /// <returns>Converted int with the concatenated suffix</returns>
    public static string ToStringWithSuffix(this int input, string suffix)
    {
        return $"{input}{suffix}";
    }

    /// <summary>
    /// Converts an int to string with a concatenated value.
    /// </summary>
    /// <param name="input">Int to convert</param>
    /// <param name="suffix">Suffix to concatentate</param>
    /// <returns>Converted int with the concatenated suffix</returns>
    public static string ToStringWithSuffix(this double input, string suffix)
    {
        return $"{input}{suffix}";
    }

    /// <summary>
    /// Converts to a double from an int as hundredths
    /// </summary>
    /// <param name="input">Int as hundredths</param>
    /// <returns>Double value</returns>
    public static double FromHundredths(this int input)
    {
        return Math.Round(input / 100.0, 2);
    }

    /// <summary>
    /// Converts to a double from an int as tenths
    /// </summary>
    /// <param name="input">Int as tenths</param>
    /// <returns>Double value</returns>
    public static double FromTenths(this int input)
    {
        return Math.Round(input / 10.0, 1);
    }
}
