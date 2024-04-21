using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

ActivitySource source = new("AspireWithSeq.ApiService.Weather");

app.MapGet("/weatherforecast", () =>
{
    var minTemperature = -20;
    var maxTemperature = 55;

    // Start an activity to log weather forecast calculation
    using var activity = source.StartActivity("Calculating weather forecasts between {MinTemperature} and {MaxTemperature}");

    // Set tags for minimum and maximum temperatures
    activity?.SetTag("MinTemperature", minTemperature);
    activity?.SetTag("MaxTemperature", maxTemperature);

    // Simulate some processing time (e.g., fetching data from an external service)
    System.Threading.Thread.Sleep(Random.Shared.Next(1000));

    // Generate a weather forecast for the next 5 days
    var forecasts = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(minTemperature, maxTemperature),
            GetRandomSummary()
        ))
        .ToArray();

    return forecasts;
});

// Helper method to get a random weather summary
string GetRandomSummary()
{
    var summaries = new[]
    {
        "Sunny", "Cloudy", "Rainy", "Snowy", "Windy"
    };
    return summaries[Random.Shared.Next(summaries.Length)];
}



app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
