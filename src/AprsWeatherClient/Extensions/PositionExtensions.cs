using AprsSharp.Parsers.Aprs;

namespace AprsWeatherClient.Extensions;

/// <summary>
/// Extenion methods for the <see cref="Position"/> class.
/// </summary>
public static class PositionExtensions
{
    private static readonly SortedDictionary<double, string> directionDictionary = new SortedDictionary<double, string>()
        {
            { 11.25, "N" },
            { 33.75, "NNE" },
            { 56.25, "NE" },
            { 78.75, "ENE" },
            { 101.25, "E" },
            { 123.75, "ESE" },
            { 146.25, "SE" },
            { 168.75, "SSE" },
            { 191.25, "S" },
            { 213.75, "SSW" },
            { 236.25, "SW" },
            { 258.75, "WSW" },
            { 281.25, "W" },
            { 303.75, "WNW" },
            { 326.25, "NW" },
            { 348.75, "NNW" },
            { 360, "N" },
        };


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

        return Math.Round((reportPosition.Coordinates.GetDistanceTo(userPosition.Coordinates) / 1000.0), 2);
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

        var direction = Math.Atan2(
            reportPosition.Coordinates.Latitude - userPosition.Coordinates.Latitude,
            reportPosition.Coordinates.Longitude - userPosition.Coordinates.Longitude);

        // ensure direction is positive
        direction = (direction + 360.0) % 360.0;

        return directionDictionary.First(entry => direction < entry.Key).Value;
    }
}
