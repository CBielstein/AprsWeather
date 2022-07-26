using AprsSharp.Parsers.Aprs;
using AprsWeatherClient.Extensions;
using Xunit;

namespace AprsWeatherServerTests;

/// <summary>
/// Tests extensions for the <see cref="Position"/> class
/// </summary>
public class PositionExtensionsTests
{
    /// <summary>
    /// Validates finding direction from a user to a report
    /// </summary>
    /// <param name="user">Gridsquare of the "user"</param>
    /// <param name="report">Gridsquare of the "report"</param>
    /// <param name="expectedDirection">The expected direction result</param>
    [Theory]
    [InlineData("KI88mm", "KJ80mm", "N")]
    [InlineData("KI88mm", "JK90", "NNE")]
    [InlineData("KI88mm", "LJ00", "NE")]
    [InlineData("KI88mm", "LI09", "ENE")]
    [InlineData("KI88mm", "LI08mm", "E")]
    [InlineData("KI88mm", "LI07", "ESE")]
    [InlineData("KI88mm", "LI06", "SE")]
    [InlineData("KI88mm", "KI96", "SSE")]
    [InlineData("KI88mm", "KI86mm", "S")]
    [InlineData("KI88mm", "KI76", "SSW")]
    [InlineData("KI88mm", "KI66", "SW")]
    [InlineData("KI88mm", "KI67", "WSW")]
    [InlineData("KI88mm", "KI68mm", "W")]
    [InlineData("KI88mm", "KI69", "WNW")]
    [InlineData("KI88mm", "KJ60", "NW")]
    [InlineData("KI88mm", "KJ70", "NNW")]
    public void TestDirectionTo(string user, string report, string expectedDirection)
    {
        var userPosition = new Position();
        userPosition.DecodeMaidenhead(user);
        var reportPosition = new Position();
        reportPosition.DecodeMaidenhead(report);
        var actualDirection = userPosition.DirectionTo(reportPosition);
        Assert.Equal(expectedDirection, actualDirection);
    }
}
