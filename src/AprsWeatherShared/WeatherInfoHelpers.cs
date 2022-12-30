using AprsSharp.Parsers.Aprs;

namespace AprsWeather.Shared;

/// <summary>
/// Resources for handling <see cref="WeatherInfo"/> packets.
/// </summary>
public static class WeatherInfoHelpers
{
    /// <summary>
    /// Deletage for mapping a <see cref="WeatherInfo"/> packet to a display
    /// string for a specific measurement property.
    /// </summary>
    /// <param name="wi">The <see cref="WeatherInfo"/> from which to take the measurement.</param>
    /// <returns>A string for display in the UI.</returns>
    public delegate string? MeasurementDisplayMap(WeatherInfo wi);

    /// <summary>
    /// A list of labels and mapping functions for what to display in the UX.
    /// </summary>
    public static readonly List<(string, MeasurementDisplayMap)> PropertyLabels = new List<(string, MeasurementDisplayMap)>()
    {
        ("Temperature", wi => wi.Temperature?.ToStringWithSuffix("F")),
        ("Rainfall Since Midnight", wi => wi.RainfallSinceMidnight?.FromHundredths().ToStringWithSuffix("\"")),
        ("24 Hour Rainfall", wi => wi.Rainfall24Hour?.FromHundredths().ToStringWithSuffix("\"")),
        ("1 Hour Rainfall", wi => wi.Rainfall1Hour?.FromHundredths().ToStringWithSuffix("\"")),
        ("Windspeed", wi => wi.WindSpeed?.ToStringWithSuffix(" MPH")),
        ("Wind Direction", wi => wi?.WindDirection?.ToCardinalDirection()),
        ("Wind Gust", wi => wi.WindGust?.ToStringWithSuffix(" MPH")),
        ("Humidity", wi => wi.Humidity?.ToStringWithSuffix("%")),
        ("Barometric Pressure", wi => wi.BarometricPressure?.FromTenths().ToStringWithSuffix(" mbar")),
        ("24 Hour Snowfall", wi => wi.Snow?.ToStringWithSuffix("\"")),
        ("Luminosity", wi => wi.Luminosity?.ToStringWithSuffix(" watts/m^2")),
    };
}
