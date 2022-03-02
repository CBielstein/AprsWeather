using Microsoft.AspNetCore.Mvc;

namespace AprsWeatherServer.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherReportsController : ControllerBase
{
    private readonly ILogger<WeatherReportsController> logger;
    private readonly IDictionary<string, string> reports;

    public WeatherReportsController(ILogger<WeatherReportsController> logger, IDictionary<string, string> reports)
    {
        this.logger = logger;
        this.reports = reports;
    }

    [HttpGet(Name = "GetWeatherReports")]
    public IEnumerable<string> Get()
    {
        return reports.Values;
    }
}
