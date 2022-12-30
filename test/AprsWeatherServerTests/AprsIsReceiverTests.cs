using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AprsSharp.Parsers.Aprs;
using AprsWeather.Shared;
using AprsWeatherServer.BackgroundServices;
using Xunit;

namespace AprsWeatherServerTests;

public class AprsIsReceiverTests
{
    public readonly IDictionary<string, WeatherReport> serverReports = new Dictionary<string, WeatherReport>();

    /// <summary>
    /// Verifies that <see cref="AprsIsReceiver"/> excludes any reports that are
    /// not populated <see cref="WeatherInfo"/> packets
    /// </summary>
    /// <param name="encodedPacket">The encoded packet as it would come from the APRS-IS server.</param>
    /// <param name="expectedInclude">Expected include result</param>
    [Theory]
    [InlineData(@"N0CALL>WIDE1-1,WIDE2-2:/092345z4903.50N/07201.75W_180/010g015t068r001p011P010h99b09901l010#010s050 Testing WX packet.", true)]
    [InlineData(@"N0CALL>WIDE1-1,WIDE2-2:/092345z4903.50N/07201.75W_t068 Testing WX with only temp.", true)]
    [InlineData(@"N0CALL>WIDE1-1,WIDE2-2:/092345z4903.50N/07201.75W_180/010 Testing WX with only wind direction and speed.", true)]
    [InlineData(@"N0CALL>WIDE1-1,WIDE2-2:/092345z4903.50N/07201.75W_180h99b0990 Testing WX with only humidity and pressure.", true)]
    [InlineData(@"N0CALL>WIDE1-1,WIDE2-2:/092345z4903.50N/07201.75W_ Testing WX with no data.", false)]
    [InlineData(@"N0CALL>WIDE1-1,igate,TCPIP*:/092345z4903.50N/07201.75W>Test1234", false)]
    public void IncludeReport(string encodedPacket, bool expectedInclude)
    {
        var packet = new Packet(encodedPacket);
        Assert.Equal(expectedInclude, AprsIsReceiver.ShouldStoreReport(packet));
    }
}
