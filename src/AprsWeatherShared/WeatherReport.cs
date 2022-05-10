namespace AprsWeather.Shared;

/// <summary>
/// Used to store and transfer APRS weather reports and related information.
/// </summary>
/// <typeparam name="T">
///     The type of the report.
///     Likely <see cref="string"/> or an AprsSharp Packet.
/// </typeparam>
public class WeatherReport<T>
{
    /// <summary>
    /// Represents the time this <see cref="WeatherReport"/>
    /// was received by the server.
    /// </summary>
    public DateTimeOffset ReceivedTime { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The report itself.
    /// Likely a <see cref="string"/> encoded packet or AprsSharp Packet object.
    /// </summary>
    public T Report { get; set; } = default!;
}
