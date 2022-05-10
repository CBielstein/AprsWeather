using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AprsSharp.Parsers.Aprs;
using AprsWeather.Shared;
using AprsWeatherServer.BackgroundServices;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AprsWeatherServerTests;

public class WeatherReportsControllerTests
{
    public readonly IDictionary<string, WeatherReport<string>> serverReports = new Dictionary<string, WeatherReport<string>>();
    public readonly HttpClient client;

    public WeatherReportsControllerTests()
    {
        var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder
                    .ConfigureTestServices(services =>
                    {
                        services.AddSingleton(serverReports);

                        var receiver = services.Single(s => s.ImplementationType == typeof(AprsIsReceiver));
                        services.Remove(receiver);

                        var expiry = services.Single(s => s.ImplementationType == typeof(ReportExpiry));
                        services.Remove(expiry);
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
    public async Task TestGetAllReportsEmpty()
    {
        serverReports.Clear();

        var reports = await GetReports();
        Assert.Empty(reports);
    }

    /// <summary>
    /// Gets <see cref="WeatherReport"/>s from the test server
    /// </summary>
    /// <param name="expected"></param>
    /// <returns>List of <see cref="WeatherReport"/>.</returns>
    private async Task<IEnumerable<WeatherReport<Packet>>> GetReports(HttpStatusCode expectedStatus = HttpStatusCode.OK)
    {
        // var response = await client.GetAsync<IEnumerable<WeatherReport<string>>>("/WeatherReports");
        var response = await client.GetAsync("/WeatherReports");
        Assert.Equal(expectedStatus, response.StatusCode);

        var reports = await response.Content.ReadFromJsonAsync<IEnumerable<WeatherReport<string>>>()
            ?? Enumerable.Empty<WeatherReport<string>>();

        return reports.Select(r =>
            new WeatherReport<Packet>()
            {
                Report = new Packet(r.Report),
                ReceivedTime = r.ReceivedTime,
            });
    }
}