using AprsSharp.Connections.AprsIs;
using AprsSharp.Parsers.Aprs;

namespace AprsWeatherServer.BackgroundServices;

public class AprsIsReceiver: IHostedService
{
    private readonly IDictionary<string, string> reports = new Dictionary<string, string>();
    private Task? receiveTask;
    private readonly AprsIsConnection client = new AprsIsConnection(new TcpConnection());

    public AprsIsReceiver(IDictionary<string, string> reports)
    {
        this.reports = reports;
        client.ReceivedPacket += p =>
        {
            if (p.InfoField is WeatherInfo)
            {
                reports[p.Sender] = p.Encode();
            }
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        string callsign = Environment.GetEnvironmentVariable("APRS_IS_CALLSIGN") ?? throw new ArgumentException("APRS_IS_CALLSIGN environment variable must be set.");

        // Return any packets (less non-position types) within 50 km of Seattle's Space Needle
        receiveTask = client.Receive(callsign, "-1", "noam.aprs2.net", "r/47.620157/-122.349643/50 -t/oimqstu");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return receiveTask ?? Task.CompletedTask;
    }
}
