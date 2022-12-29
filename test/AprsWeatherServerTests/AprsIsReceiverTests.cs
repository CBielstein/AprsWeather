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
    /// Verifies that <see cref="AprsIsReceiver"/> excludes any reports that are not <see cref="WeatherInfo"/>
    /// </summary>
    [Fact]
    public Task ExcludeNonWeatherReport()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Verifies that <see cref="AprsIsReceiver"/> excludes any reports that do not include
    /// any of our known <see cref="WeatherInfo"/> properties
    /// </summary>
    [Fact]
    public Task ExcludeEmptyWeatherReport()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Verifies that <see cref="WeatherInfo"/> packets are correctly received and saved
    /// </summary>
    [Fact]
    public Task ReceiveWeatherReport()
    {
        throw new NotImplementedException();
    }
}
