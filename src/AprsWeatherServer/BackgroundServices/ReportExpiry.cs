using AprsSharp.Parsers.Aprs;

namespace AprsWeatherServer.BackgroundServices;

/// <summary>
/// Periodically deletes weather reports that have expired.
/// </summary>
public class ReportExpiry: IHostedService
{
    private readonly IDictionary<string, string> reports;
    private readonly ILogger<ReportExpiry> logger;
    private Task? workerTask;
    private readonly CancellationTokenSource workerTokenSource = new CancellationTokenSource();
    private readonly int reportExpiryMinutes;
    private readonly int taskFrequencyMs;

    public ReportExpiry(
        IDictionary<string, string> reports,
        IConfiguration configuration,
        ILogger<ReportExpiry> logger)
    {
        this.reports = reports;
        this.logger = logger;
        reportExpiryMinutes = int.Parse(configuration["ReportExpiryMinutes"]);
        taskFrequencyMs = int.Parse(configuration["ExpiryCheckFrequencyMinutes"]) * 60 * 1000;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var token = workerTokenSource.Token;
        workerTask = Task.Run(() => RemoveExpiredReports(token), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        workerTokenSource.Cancel();
        return workerTask ?? Task.CompletedTask;
    }

    private async Task RemoveExpiredReports(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            IEnumerable<string> expiredKeys = reports
                .Where(r => 
                {
                    var p = new Packet(r.Value);
                    var packetTime = (p.InfoField as WeatherInfo)?.Timestamp?.DateTime.ToUniversalTime();
                    var currentTimeAlive = packetTime?.Subtract(DateTime.UtcNow).Duration();
                    return currentTimeAlive > TimeSpan.FromMinutes(reportExpiryMinutes);
                })
                .Select(r => r.Key);

            logger.LogInformation("Removing the following keys: {keys}", string.Join(',', expiredKeys));

            foreach (var expiredKey in expiredKeys)
            {
                reports.Remove(expiredKey);
            }

            await Task.Delay(taskFrequencyMs, stoppingToken);
        }
    }
}
