using AprsSharp.Parsers.Aprs;
using AprsWeather.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AprsWeatherServer.Controllers;

[ApiController]
[Route("[controller]/[action]")]
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
    /// Gets <see cref="WeatherReport"/>s near a given location.
    /// </summary>
    /// <param name="location">Sorts by the proximity to a given location (specified as the centerpoint of a gridsquare).</param>
    /// <param name="limit">Limits number of reports to the number given, default is 1.</param>
    /// <param name="skip">Skips ordered reports, used for paging. Default is 0.</param>
    /// <returns><see cref="WeatherReport"/>s filtered and limited as requested.</returns>
    [HttpGet(Name = "GetWeatherReportsNearLocation")]
    public IEnumerable<WeatherReport> Near(
        [FromQuery] string location,
        [FromQuery] int limit = 1,
        [FromQuery] int skip = 0)
    {
        if (string.IsNullOrEmpty(location))
        {
            throw new ArgumentNullException(nameof(location));
        }
        else if (limit < 1 || limit > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), $"{nameof(limit)} should be in range [1, 10]");
        }

        var locationPosition = new Position();
        locationPosition.DecodeMaidenhead(location);

        return reports.Values
            .OrderBy(r => (r.Packet.InfoField as WeatherInfo)?.Position.Coordinates.GetDistanceTo(locationPosition.Coordinates))
            .Skip(skip)
            .Take(limit);
    }

    /// <summary>
    /// Returns the total number of <see cref="WeatherReport"/>s currently held by the server.
    /// </summary>
    /// <returns>The count of currently-held <see cref="WeatherReport"/>s.</returns>
    [HttpGet(Name = "GetTotalWeatherReportCount")]
    public int Count()
    {
        return reports.Count;
    }
}
