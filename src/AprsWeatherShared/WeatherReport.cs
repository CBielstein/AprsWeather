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
    [JsonConverter(typeof(PacketJsonConverter))]
    public Packet Packet { get; set; } = default!;
}
