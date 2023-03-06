using System.Text.RegularExpressions;
using AprsSharp.Parsers.Aprs;
using AprsWeatherClient.Models;
using Darnton.Blazor.DeviceInterop.Geolocation;
using Microsoft.AspNetCore.Components;

namespace AprsWeatherClient.Pages;

public partial class Index : ComponentBase
{
    /// <summary>
    /// The value of the location textbox
    /// </summary>
    private string? userGridsquare;

    /// <summary>
    /// The saved position of the user for calculating distances
    /// </summary>
    private Position? userPosition;

    /// <summary>
    /// Output to user in case of errors
    /// </summary>
    private string userMessage = string.Empty;

    /// <summary>
    /// Case-insensitive gridsquare of length 4, 6, or 8
    /// </summary>
    private const string GRIDSQUARE_REGEX = @"^[a-zA-Z]{2}[0-9]{2}(([a-zA-Z]{2})|([a-zA-Z]{2}[0-9]{2}))?$";

    /// <summary>
    /// A <see cref="System.Threading.Timer"/> to update the "minutes ago" UI.
    /// </summary>
    private readonly Timer agoUpdateTimer;

    /// <summary>
    /// The frequency to update the "minutes ago" UI.
    /// Go with every 30 seconds as the redraw should be pretty lightweight and means we'll be closer to up-to-date than a full minute.
    /// </summary>
    private const int agoUpdateInterval = 30000;

    /// <summary>
    /// Location service to query user device location
    /// </summary>
    [Inject]
    public IGeolocationService LocationService { get; set; } = default!;

    /// <summary>
    /// The object to manage <see cref="WeatherReport"/>s and fetching from the server
    /// </summary>
    [Inject]
    public ReportList ReportList { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Index"/> class.
    /// </summary>
    public Index()
    {
        agoUpdateTimer = new Timer(_ => this.StateHasChanged(), null, agoUpdateInterval, agoUpdateInterval);
    }

    /// <summary>
    /// Sets the location using a value from the examples dropdown.
    /// </summary>
    /// <param name="args">Event change args</param>
    /// <returns>The asynchronous task</returns>
    private Task SetExampleLocation(ChangeEventArgs args)
    {
        userGridsquare = args.Value as string ?? throw new Exception("HTML select object did not have value");
        return ManualLocation();
    }

    /// <summary>
    /// Sets the location using a manual entry value.
    /// </summary>
    /// <returns>The asynchronous task</returns>
    private Task ManualLocation()
    {
        if (!Regex.IsMatch(userGridsquare ?? string.Empty, GRIDSQUARE_REGEX))
        {
            return Task.CompletedTask;
        }

        userPosition = new Position();

        // Null checked above in the regex match.
        userPosition.DecodeMaidenhead(userGridsquare!);

        return LoadNewReports();
    }

    /// <summary>
    /// Sets the location using the geolocation API.
    /// </summary>
    /// <returns>The asynchronous task</returns>
    private async Task AutoLocation()
    {
        GeolocationResult location = await LocationService.GetCurrentPosition();

        if (!location.IsSuccess)
        {
            userMessage = "Unable to retrieve user location.";
            return;
        }

        userPosition = new Position();
        userPosition.Coordinates = new GeoCoordinatePortable.GeoCoordinate(location.Position.Coords.Latitude, location.Position.Coords.Longitude);
        userGridsquare = userPosition.EncodeGridsquare(6, false);

        await LoadNewReports();
    }

    /// <summary>
    /// Loads new reports using the current value of <see cref="userGridsquare"/>
    /// </summary>
    /// <returns>The asynchronous task</returns>
    private async Task LoadNewReports()
    {
        if (!Regex.IsMatch(userGridsquare ?? string.Empty, GRIDSQUARE_REGEX))
        {
            return;
        }

        userMessage = string.Empty;

        // Null checked above in the regex match.
        await ReportList.SetLocation(userGridsquare!);
    }

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        return LoadNewReports();
    }
}
