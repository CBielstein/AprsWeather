using AprsSharp.Connections.AprsIs;
using AprsSharp.Parsers.Aprs;
using AprsWeather.Shared;

namespace AprsWeatherServer.BackgroundServices;

public class AprsIsReceiver: IHostedService
{
    private readonly IDictionary<string, WeatherReport<string>> reports;
    private readonly ILogger<AprsIsReceiver> logger;
    private readonly ILogger<AprsIsClient> clientLogger;

    private Task? receiveTask;
    private AprsIsClient? client;
    private bool attemptReconnect = true;

    // Return any packets (less non-position types) within 50 km of Seattle's Space Needle
    private const string filter = "r/47.620157/-122.349643/50 -t/oimqstu";
    private readonly string callsign = Environment.GetEnvironmentVariable("APRS_IS_CALLSIGN")
        ?? throw new ArgumentException("APRS_IS_CALLSIGN environment variable must be set.");
    private const string password = "-1";
    private const string server = "noam.aprs2.net";

    public AprsIsReceiver(
        IDictionary<string, WeatherReport<string>> reports,
        ILogger<AprsIsReceiver> logger,
        ILogger<AprsIsClient> clientLogger)
    {
        this.reports = reports;
        this.logger = logger;
        this.clientLogger = clientLogger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting service: {serviceName}", nameof(AprsIsReceiver));
        CreateConnection(ConnectionState.Disconnected);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping service: {serviceName}", nameof(AprsIsReceiver));
        attemptReconnect = false;
        client?.Disconnect();
        return receiveTask ?? Task.CompletedTask;
    }

    private void CreateConnection(ConnectionState newState)
    {
        logger.LogInformation("APRS-IS connection entered new state: {state}", newState);

        if (client != null && newState != ConnectionState.Disconnected)
        {
            return;
        }

        if (!attemptReconnect)
        {
            logger.LogInformation("Shutting down. Not attempting APRS-IS reconnection.");
            return;
        }

        if (client != null)
        {
            logger.LogWarning("APRS-IS connection failed. Reconnecting.");
            Thread.Sleep(15000);
        }

        logger.LogInformation("Creating new {AprsIsClient}", nameof(AprsIsClient));

        client = new AprsIsClient(clientLogger);

        client.ReceivedPacket += StorePacket;
        client.ChangedState += CreateConnection;

        receiveTask = client.Receive(callsign, password, server, filter);
    }

    private void StorePacket(Packet p)
    {
        if (p.InfoField is WeatherInfo)
        {
            reports[p.Sender] = new WeatherReport<string>()
            {
                ReceivedTime = DateTimeOffset.UtcNow,
                Report = p.Encode(),
            };
        }
    }
}
