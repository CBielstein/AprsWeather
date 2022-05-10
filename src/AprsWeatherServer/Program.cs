using AprsWeather.Shared;
using AprsWeatherServer.BackgroundServices;

var devCorsName = "DevelopmentCorsPolicy";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IDictionary<string, WeatherReport<string>>>(
    new Dictionary<string, WeatherReport<string>>());

builder.Services.AddHostedService<AprsIsReceiver>();
builder.Services.AddHostedService<ReportExpiry>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: devCorsName,
        b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

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
}

app.UseHttpsRedirection();

// Disable CORS in development
if (app.Environment.IsDevelopment())
{
    app.UseCors(devCorsName);
}

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make this class public for use in tests
public partial class Program { }
