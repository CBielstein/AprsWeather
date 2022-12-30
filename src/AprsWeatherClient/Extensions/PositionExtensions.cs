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
}
