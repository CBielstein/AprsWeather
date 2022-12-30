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
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AprsWeatherServerTests;

public class WeatherReportsControllerTests
{
    public readonly IDictionary<string, WeatherReport> serverReports = new Dictionary<string, WeatherReport>();
    public readonly HttpClient client;
    private const string reportsNearEndpoint = "WeatherReports/Near";

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
    /// Verifies that the limit parameter is respected and changes how many packets are returned.
    /// </summary>
    /// <param name="limit">Limit on reports to request.</param>
    /// <param name="expectedCode">Expected response code from the server, allowing testing of success and failures.</param>
    [Theory]
    [InlineData(3, HttpStatusCode.OK)]
    [InlineData(5, HttpStatusCode.OK)]
    [InlineData(1, HttpStatusCode.OK)]
    [InlineData(0, HttpStatusCode.InternalServerError)] // TODO: Issue 14
    [InlineData(100, HttpStatusCode.InternalServerError)] // TODO: Issue 14
    public async Task TestLimitReports(int limit, HttpStatusCode expectedCode)
    {
        var serverReports = new[]
        {
            @"N0CALL-1>WIDE2-2:/092345z4903.50N/07201.75W_180/010g015t068r001p011P010h99b09901l010#010s050 Testing WX packet.",
            @"N0CALL-2>WIDE1-1:/092345z4903.50N/07201.75W_180/010 Testing WX packet #2.",
            @"N0CALL-3>WIDE1-1:/092345z4903.50N/07201.75W_180/010 Testing WX packet #3.",
            @"N0CALL-4>WIDE1-1:/092345z4903.50N/07201.75W_180/010 Testing WX packet #4.",
            @"N0CALL-5>WIDE1-1:/092345z4903.50N/07201.75W_180/010 Testing WX packet #5.",
        };
        SetServerReports(serverReports);

        var args = new Dictionary<string, string?>();
        args.Add("limit", limit.ToString());
        args.Add("location", "JJ00aa");

        var reports = await GetReports(reportsNearEndpoint, args: args, expectedStatus: expectedCode);

        if (expectedCode == HttpStatusCode.InternalServerError)
        {
            Assert.Null(reports);
            return;
        }

        // Can't be more reports returned than what the server holds
        Assert.Equal(Math.Min(limit, serverReports.Length), reports!.Count());
    }

    /// <summary>
    /// Verifies that the location parameter is used to order the returned packets.
    /// </summary>
    /// <param name="limit">Test variations in limit with order by location.</param>
    /// <param name="expectedCode">Expected response code from the server, allowing testing of success and failures.</param>
    [Theory]
    [InlineData(null, HttpStatusCode.OK)]
    [InlineData(0, HttpStatusCode.InternalServerError)] // TODO: Issue 14
    [InlineData(11, HttpStatusCode.InternalServerError)] // TODO: Issue 14
    [InlineData(3, HttpStatusCode.OK)]
    [InlineData(1, HttpStatusCode.OK)]
    public async Task TestOrderByLocation(int? limit, HttpStatusCode expectedCode)
    {
        // The last character of the callsign (N0CALL-x) is the expected ordering
        var serverReports = new[]
        {
            @"N0CALL-5>WIDE1-1:/092345z1000.00S/01000.00W_180/010",
            @"N0CALL-3>WIDE1-1:/092345z0010.00S/00010.00W_180/010",
            @"N0CALL-1>WIDE1-1:/092345z0001.00N/00010.00W_180/010",
            @"N0CALL-4>WIDE1-1:/092345z0100.00S/00100.00E_180/010",
            @"N0CALL-2>WIDE1-1:/092345z0001.50N/00010.50W_180/010",
        };
        SetServerReports(serverReports);

        var args = new Dictionary<string, string?>();
        args.Add("location", "JJ00aa");

        // Test default limit == 1
        if (limit != null)
        {
            args.Add("limit", limit.ToString());
        }

        var reports = await GetReports(reportsNearEndpoint, args: args, expectedStatus: expectedCode);

        if (expectedCode == HttpStatusCode.InternalServerError)
        {
            Assert.Null(reports);
            return;
        }

        Assert.Equal(limit ?? 1, reports!.Count());

        // Assert the ordering is correct
        for (var i = 1; i <= (limit ?? reports!.Count()); ++i)
        {
            var number = reports!.ElementAt(i - 1).Packet.Sender.Last();
            Assert.Equal(i, char.GetNumericValue(number));
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(0)]
    [InlineData(30)]
    public async Task TestCount(int count)
    {
        var reports = new string[count];

        for (int i = 0; i < count; ++i)
        {
            reports[i] = $"N0CALL-{i}>WIDE2-2:/092345z4903.50N/07201.75W_180/010 Testing WX packet #{i}.";
        }

        SetServerReports(reports);

        var response = await client.GetAsync("/WeatherReports/Count");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var serverCount = await response.Content.ReadAsStringAsync();
        Assert.Equal(count, int.Parse(serverCount));
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
                    new WeatherReport()
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
    /// <param name="location">Location paramater to pass with request.</param>
    /// <returns>List of <see cref="WeatherReport"/>.</returns>
    private async Task<IEnumerable<WeatherReport>?> GetReports(
        string endpoint,
        HttpStatusCode expectedStatus = HttpStatusCode.OK,
        IDictionary<string, string?>? args = null)
    {
        var request = endpoint;

        if (args != null)
        {
            request = QueryHelpers.AddQueryString(endpoint, args);
        }

        var response = await client.GetAsync(request);
        Assert.Equal(expectedStatus, response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<IEnumerable<WeatherReport>>();
    }
}
