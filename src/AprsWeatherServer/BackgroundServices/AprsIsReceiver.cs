using AprsSharp.Connections.AprsIs;
using AprsSharp.Parsers.Aprs;
using AprsWeather.Shared;

namespace AprsWeatherServer.BackgroundServices;

public class AprsIsReceiver: IHostedService
{
    private readonly IDictionary<string, WeatherReport> reports;
    private readonly ILogger<AprsIsReceiver> logger;
    private readonly ILogger<AprsIsClient> clientLogger;
    private readonly IConfiguration configuration;

    private Task? receiveTask;
    private AprsIsClient? client;
    private bool attemptReconnect = true;

    private readonly string callsign = Environment.GetEnvironmentVariable("APRS_IS_CALLSIGN")
        ?? throw new ArgumentException("APRS_IS_CALLSIGN environment variable must be set.");
    private const string password = "-1";
    private const string server = "noam.aprs2.net";
    private static readonly IEnumerable<WeatherInfoHelpers.MeasurementDisplayMap> MeasurementMaps = WeatherInfoHelpers.PropertyLabels.Select(pl => pl.Item2);

    public AprsIsReceiver(
        IDictionary<string, WeatherReport> reports,
        ILogger<AprsIsReceiver> logger,
        ILogger<AprsIsClient> clientLogger,
        IConfiguration configuration)
    {
        this.reports = reports;
        this.logger = logger;
        this.clientLogger = clientLogger;
        this.configuration = configuration;
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

        string filter = configuration["AprsIsServerFilter"];
        logger.LogInformation("Creating new {AprsIsClient} with filter {filter}", nameof(AprsIsClient), filter);

        client = new AprsIsClient(clientLogger);

        client.ReceivedTcpMessage += ProcessReport;
        client.ChangedState += CreateConnection;

        receiveTask = client.Receive(callsign, password, server, filter);
    }

    /// <summary>
    /// Processes incoming <see cref="Packet"/> and stores any appropriate
    /// <see cref="WeatherInfo"/> packets.
    /// </summary>
    /// <param name="report">The encoded string received from the server.</param>
    private void ProcessReport(string encodedPacket)
    {
        try
        {
            var p = new Packet(encodedPacket);

            if (ShouldStoreReport(p))
            {
                reports[p.Sender] = new WeatherReport()
                {
                    Report = encodedPacket,
                };
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to decode received packet: {encodedPacket}", encodedPacket);
        }
    }

    /// <summary>
    /// Determines if a <see cref="Packet"/> should be saved as a relevant
    /// <see cref="WeatherReport"/> by finding if there are any values we
    /// would display in the client.
    /// </summary>
    /// <param name="packet">The <see cref="Packet"/> to check.</param>
    /// <returns>True if the <see cref="Packet"/> should be saved as a <see cref="WeatherReport"/>.</returns>
    public static bool ShouldStoreReport(Packet packet)
    {
        return (packet.InfoField is WeatherInfo wi) &&
                MeasurementMaps.Any(displayMap => displayMap(wi) != null);
    }
}
