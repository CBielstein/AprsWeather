using AprsSharp.Parsers.Aprs;
using Geolocation;

namespace AprsWeatherClient.Extensions;

/// <summary>
/// Extenion methods for positions and related concepts.
/// </summary>
public static class PositionExtensions
{
    /// <summary>
    /// Calculates a distance from a user position to a report position
    /// </summary>
    /// <param name="userPositions"><see cref="Position"/> of the user</param>
    /// <param name="reportPosition"><see cref="Position"/> of the report</param>
    /// <returns>Distance in miles</returns>
    public static double? MilesTo(this Position? userPosition, Position reportPosition)
    {
        if (userPosition == null)
        {
            return null;
        }

        return GeoCalculator.GetDistance(
            userPosition.Coordinates.Latitude,
            userPosition.Coordinates.Longitude,
            reportPosition.Coordinates.Latitude,
            reportPosition.Coordinates.Longitude);
    }

    /// <summary>
    /// Calculates a direction from a user position to a report position
    /// </summary>
    /// <param name="userPositions"><see cref="Position"/> of the user</param>
    /// <param name="reportPosition"><see cref="Position"/> of the report</param>
    /// <returns>Direction string (e.g. NW, E, etc.)</returns>
    public static string? DirectionTo(this Position? userPosition, Position reportPosition)
    {
        if (userPosition == null)
        {
            return null;
        }

        return GeoCalculator.GetDirection(
            userPosition.Coordinates.Latitude,
            userPosition.Coordinates.Longitude,
            reportPosition.Coordinates.Latitude,
            reportPosition.Coordinates.Longitude);
    }

    /// <summary>
    /// Map of upper bound of degrees and the cardinal direction it represents.
    /// Referenced from http://snowfence.umn.edu/Components/winddirectionanddegrees.htm
    /// </summary>
    private static List<(double, string)> directionBounds = new List<(double, string)>()
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
