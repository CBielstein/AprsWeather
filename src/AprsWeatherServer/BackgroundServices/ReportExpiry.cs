using AprsSharp.Parsers.Aprs;
using AprsWeather.Shared;

namespace AprsWeatherServer.BackgroundServices;

/// <summary>
/// Periodically deletes weather reports that have expired.
/// </summary>
public class ReportExpiry: IHostedService
{
    private readonly IDictionary<string, WeatherReport> reports;
    private readonly ILogger<ReportExpiry> logger;
    private readonly TimeSpan reportExpiry;
    private readonly int taskFrequencyMs;
    private readonly Timer timer;

    public ReportExpiry(
        IDictionary<string, WeatherReport> reports,
        IConfiguration configuration,
        ILogger<ReportExpiry> logger)
    {
        this.reports = reports;
        this.logger = logger;

        var reportExpiryMinutes = int.Parse(configuration["ReportExpiryMinutes"]);
        reportExpiry = TimeSpan.FromMinutes(reportExpiryMinutes);

        taskFrequencyMs = int.Parse(configuration["ExpiryCheckFrequencyMinutes"]) * 60 * 1000;

        timer = new Timer((object? _ ) => RemoveExpiredReports(), null, Timeout.Infinite, Timeout.Infinite);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        timer.Change(taskFrequencyMs, taskFrequencyMs);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        timer.Change(Timeout.Infinite, Timeout.Infinite);
        return Task.CompletedTask;
    }

    private void RemoveExpiredReports()
    {
        IEnumerable<string> expiredKeys = reports
            .Where(r =>
            {
                var received = r.Value.ReceivedTime;
                var currentTimeAlive = received.Subtract(DateTimeOffset.UtcNow).Duration();
                return currentTimeAlive > reportExpiry;
            })
            .Select(r => r.Key);

        logger.LogInformation("Removing the following keys: {keys}", string.Join(',', expiredKeys));

        foreach (var expiredKey in expiredKeys)
        {
            reports.Remove(expiredKey);
        }
    }
}
