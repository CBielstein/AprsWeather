using System.Collections.Generic;
using AprsWeather.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AprsWeatherServerTests;

public class WeatherReportsControllerTests
{
    public readonly IDictionary<string, WeatherReport<string>> reports = new Dictionary<string, WeatherReport<string>>();

    public WeatherReportsControllerTests()
    {
        var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b =>
            {
                b.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IDictionary<string, WeatherReport<string>>>(reports);
                });
            });
    }

    /// <summary>
    /// Verifies all reports are returned
    /// </summary>
    [Fact]
    public void TestGetAllReports()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Verifies empty list when no reports available
    /// </summary>
    [Fact]
    public void TestGetAllReportsEmpty()
    {
        throw new System.NotImplementedException();
    }
}