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
    [InlineData("KI88", "KJ80", "N")]
    [InlineData("KI88", "JK90", "NNE")]
    [InlineData("KI88", "LJ00", "NE")]
    [InlineData("KI88", "LI09", "ENE")]
    [InlineData("KI88", "LI08", "E")]
    [InlineData("KI88", "LI07", "ESE")]
    [InlineData("KI88", "LI06", "SE")]
    [InlineData("KI88", "KI96", "SSE")]
    [InlineData("KI88", "KI86", "S")]
    [InlineData("KI88", "KI76", "SSW")]
    [InlineData("KI88", "KI66", "SW")]
    [InlineData("KI88", "KI67", "WSW")]
    [InlineData("KI88", "KI68", "W")]
    [InlineData("KI88", "KI69", "WNW")]
    [InlineData("KI88", "KJ60", "NW")]
    [InlineData("KI88", "KJ70", "NNW")]
    public void TestDirectionTo(string user, string report, string expectedDirection)
    {
        var userPosition = new Position();
        userPosition.DecodeMaidenhead(user);
        var reportPosition = new Position();
        reportPosition.DecodeMaidenhead(report);
        Assert.Equal(expectedDirection, userPosition.DirectionTo(reportPosition));
    }
}
