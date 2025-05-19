using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;
using WeatherApi.Models;
using WeatherApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CityDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CitiesDb")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));


builder.Services.AddHostedService<CityDataInitializer>();

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<ICityService, CityService>();


builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalCorsPolicy", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrWhiteSpace(origin)) return false;

                var uri = new Uri(origin);
                return (uri.Host == "localhost" || uri.Host == "127.0.0.1" || uri.Host == "::1");
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("Set-Cookie");
    });
});

var app = builder.Build();

app.UseCors("LocalCorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
