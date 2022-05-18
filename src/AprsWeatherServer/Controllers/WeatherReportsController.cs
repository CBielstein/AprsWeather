using AprsWeather.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AprsWeatherServer.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherReportsController : ControllerBase
{
    private readonly ILogger<WeatherReportsController> logger;
    private readonly IDictionary<string, WeatherReport> reports;

    public WeatherReportsController(
        ILogger<WeatherReportsController> logger,
        IDictionary<string, WeatherReport> reports)
    {
        this.logger = logger;
        this.reports = reports;
    }

    /// <summary>
    /// Gets <see cref="WeatherReport"/>s held by the server.
    /// </summary>
    /// <param name="limit">Limits number of reports to the number given.</param>
    /// <param name="location">Sorts by the proximity to a given location (specified as the centerpoint of a gridsquare).</param>
    /// <returns><see cref="WeatherReport"/>s filtered or limited as requested.</returns>
    [HttpGet(Name = "GetWeatherReports")]
    public IEnumerable<WeatherReport> Get(
        [FromQuery] int? limit,
        [FromQuery] string? location)
    {
        var packets = reports.Values;

        if (location != null)
        {
            throw new NotImplementedException();
        }

        if (limit != null)
        {
            packets = packets.Take(limit.Value).ToList();
        }

        return packets;
    }
}
