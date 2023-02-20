using AprsWeather.Shared;
using AprsWeatherServer.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IDictionary<string, WeatherReport>>(
    new Dictionary<string, WeatherReport>());

builder.Services.AddHostedService<AprsIsReceiver>();
builder.Services.AddHostedService<ReportExpiry>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
}
else
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();

// Make this class public for use in tests
public partial class Program { }
