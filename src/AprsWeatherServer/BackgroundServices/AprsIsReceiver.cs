using AprsSharp.Connections.AprsIs;
using AprsSharp.Parsers.Aprs;
using AprsWeather.Shared;

namespace AprsWeatherServer.BackgroundServices;

public class AprsIsReceiver: IHostedService
{
    private readonly IDictionary<string, WeatherReport<string>> reports;
    private readonly ILogger<AprsIsReceiver> logger;
    private Task? receiveTask;
    private TcpConnection? tcpConnection;
    private AprsIsConnection? client;
    private bool attemptReconnect = true;
    private readonly string callsign = Environment.GetEnvironmentVariable("APRS_IS_CALLSIGN")
        ?? throw new ArgumentException("APRS_IS_CALLSIGN environment variable must be set.");
    private const string password = "-1";
    private const string server = "noam.aprs2.net";

    // Return any packets (less non-position types) within 50 km of Seattle's Space Needle
    private const string filter = "r/47.620157/-122.349643/50 -t/oimqstu";

    public AprsIsReceiver(
        IDictionary<string, WeatherReport<string>> reports,
        ILogger<AprsIsReceiver> logger)
    {
        this.reports = reports;
        this.logger = logger;
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
        tcpConnection?.Dispose();
        return receiveTask ?? Task.CompletedTask;
    }

    private void CreateConnection(ConnectionState newState)
    {
        logger.LogInformation("APRS-IS connection entered new state: {state}", newState);

        if (client == null || newState == ConnectionState.Disconnected)
        {
            if (client != null && newState == ConnectionState.Disconnected)
            {
                logger.LogWarning("APRS-IS connection failed.");
                Thread.Sleep(15000);
            }

            if (!attemptReconnect)
            {
                logger.LogInformation("Not attempting APRS-IS reconnection.");
                return;
            }

            logger.LogInformation("Creating new {AprsIsClient}", nameof(AprsIsConnection));

            tcpConnection?.Dispose();
            tcpConnection = new TcpConnection();
            client = new AprsIsConnection(tcpConnection);

            client.ReceivedPacket += StorePacket;
            client.ChangedState += CreateConnection;

            receiveTask = client.Receive(callsign, password, server, filter);
        }
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
