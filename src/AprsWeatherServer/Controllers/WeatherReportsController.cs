using Microsoft.AspNetCore.Mvc;

namespace AprsWeatherServer.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherReportsController : ControllerBase
{
    private readonly ILogger<WeatherReportsController> _logger;

    public WeatherReportsController(ILogger<WeatherReportsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherReports")]
    public IEnumerable<string> Get()
    {
        throw new NotImplementedException();
    }
}
