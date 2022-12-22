using System.Text.Json.Serialization;
using AprsSharp.Parsers.Aprs;

namespace AprsWeather.Shared;

/// <summary>
/// Used to store and transfer APRS weather reports and related information.
/// </summary>
public class WeatherReport
{
    /// <summary>
    /// Represents the time this <see cref="WeatherReport"/>
    /// was received by the server.
    /// </summary>
    public DateTimeOffset ReceivedTime { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The report itself.
    /// </summary>
    public string Report { get; set; } = default!;

    /// <summary>
    /// The decoded packet, if possible.
    /// </summary>
    [JsonIgnore]
    public Packet Packet => decoded ??= new Packet(Report);
    private Packet? decoded = null;
}
