namespace AprsWeather.Shared;

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


    /// <summary>
    /// Map of upper bound of degrees and the cardinal direction it represents.
    /// Referenced from http://snowfence.umn.edu/Components/winddirectionanddegrees.htm
    /// </summary>
    private static readonly List<(double, string)> directionBounds = new List<(double, string)>()
    {
        (11.25, "N"),
        (33.75, "NNE"),
        (56.25, "NE"),
        (78.75, "ENE"),
        (101.25, "E"),
        (123.75, "ESE"),
        (146.25, "SE"),
        (168.75, "SSE"),
        (191.25, "S"),
        (213.75, "SSW"),
        (236.25, "SW"),
        (258.75, "WSW"),
        (281.25, "W"),
        (303.75, "WNW"),
        (326.26, "NW"),
        (348.75, "NNW"),
        (360.1, "N"),
    };

    /// <summary>
    /// Converts an integer degrees direction to a cardinal direction
    /// </summary>
    /// <param name="">degrees heading</param>
    /// <returns>Cardinal direction heading</returns>
    public static string ToCardinalDirection(this int degrees)
    {
        if (degrees < 0 || degrees > 360)
        {
            throw new ArgumentOutOfRangeException(nameof(degrees), $"Expect degrees in [0, 360], got: {degrees}");
        }

        foreach ((double upperBound, string cardinal) in directionBounds)
        {
            if (degrees < upperBound)
            {
                return cardinal;
            }
        }

        throw new ArgumentOutOfRangeException(nameof(degrees), $"Input degrees didn't match known directions: {degrees}");
    }
}
