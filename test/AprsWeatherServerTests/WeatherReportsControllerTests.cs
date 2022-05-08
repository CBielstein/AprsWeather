using System.Collections.Generic;
using System.Net.Http;
using AprsWeather.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AprsWeatherServerTests;

public class WeatherReportsControllerTests
{
    public readonly IDictionary<string, WeatherReport<string>> reports = new Dictionary<string, WeatherReport<string>>();
    public readonly HttpClient client;

    public WeatherReportsControllerTests()
    {
        var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder
                    .ConfigureTestServices(services =>
                    {
                        services.AddSingleton(reports);
                    })
                    .UseTestServer();
            });

        client = app.CreateClient();
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