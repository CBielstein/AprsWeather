using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AprsWeatherClient;
using Darnton.Blazor.DeviceInterop.Geolocation;
using AprsWeatherClient.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var serverAddress = builder.Configuration["ServerAddress"];
ArgumentNullException.ThrowIfNull(serverAddress);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(serverAddress) });
builder.Services.AddScoped<IGeolocationService, GeolocationService>();
builder.Services.AddScoped<ReportList>();

await builder.Build().RunAsync();
