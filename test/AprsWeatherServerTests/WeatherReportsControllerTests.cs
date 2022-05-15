using System;
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
    public async Task TestGetAllReports()
    {
        var packet1 = @"N0CALL-1>WIDE2-2:/092345z4903.50N/07201.75W_180/010g015t068r001p011P010h99b09901l010#010s050 Testing WX packet.";
        var packet2 = @"N0CALL-2>WIDE1-1:/092345z4903.50N/07201.75W_180/010 Testing WX packet #2.";
        SetServerReports(new[] { packet1, packet2 });

        var response = await GetReports();

        Assert.Equal(2, response.Count());

        // Assert all reports are present
        var reports = response.Select(r => r.Report);
        Assert.Contains(packet1, reports);
        Assert.Contains(packet2, reports);

        // Assert times are within the last few minutes
        Assert.DoesNotContain(response.Select(r => r.ReceivedTime), time => DateTimeOffset.UtcNow - time > TimeSpan.FromMinutes(2));
    }

    /// <summary>
    /// Verifies empty list when no reports available
    /// </summary>
    [Fact]
    public async Task TestGetAllReportsEmpty()
    {
        SetServerReports();

        var reports = await GetReports();
        Assert.Empty(reports);
    }

    /// <summary>
    /// Verifies that the limit parameter is respected.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task TestLimitReports()
    {
        var packets = new[]
        {
            @"N0CALL-1>WIDE2-2:/092345z4903.50N/07201.75W_180/010g015t068r001p011P010h99b09901l010#010s050 Testing WX packet.",
            @"N0CALL-2>WIDE1-1:/092345z4903.50N/07201.75W_180/010 Testing WX packet #2.",
            @"N0CALL-3>WIDE1-1:/092345z4903.50N/07201.75W_180/010 Testing WX packet #3.",
            @"N0CALL-4>WIDE1-1:/092345z4903.50N/07201.75W_180/010 Testing WX packet #4.",
            @"N0CALL-5>WIDE1-1:/092345z4903.50N/07201.75W_180/010 Testing WX packet #5.",
        };
        SetServerReports(packets);

        var reports = await GetReports(limit: 3);
        Assert.Equal(3, reports.Count());
    }

    /// <summary>
    /// Resets the reports held by the server to the value given.
    /// Clears the existing reports first, so if the new list is empty, the server will
    /// have no reports.
    /// </summary>
    /// <param name="newReports">Reports to set in the test server. None, if not provided.</param>
    private void SetServerReports(IEnumerable<string>? newReports = null)
    {
        serverReports.Clear();

        if (newReports != null)
        {
            foreach (var report in newReports)
            {
                var packet = new Packet(report);

                serverReports.Add(
                    packet.Sender,
                    new WeatherReport<string>()
                    {
                        Report = report,
                    });
            }
        }
    }

    /// <summary>
    /// Gets <see cref="WeatherReport"/>s from the test server
    /// </summary>
    /// <param name="expectedStatus">Assert response code matches this.</param>
    /// <param name="limit">Limit parameter to pass with request.</param>
    /// <returns>List of <see cref="WeatherReport"/>.</returns>
    private async Task<IEnumerable<WeatherReport<string>>> GetReports(
        HttpStatusCode expectedStatus = HttpStatusCode.OK,
        int? limit = null)
    {
        var request = "/WeatherReports";

        if (limit != null)
        {
            request += $"?limit={limit}";
        }

        var response = await client.GetAsync(request);
        Assert.Equal(expectedStatus, response.StatusCode);

        return await response.Content.ReadFromJsonAsync<IEnumerable<WeatherReport<string>>>()
            ?? Enumerable.Empty<WeatherReport<string>>();
    }
}
