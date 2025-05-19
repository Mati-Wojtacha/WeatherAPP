using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using WeatherApi.Models;
using WeatherApi.Services;

public class WeatherServiceTests
{
    private readonly Mock<ILogger<WeatherService>> _loggerMock = new();
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public WeatherServiceTests()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"temp\":25}")
            });

        _httpClient = new HttpClient(handlerMock.Object);

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"OpenWeatherSettings:ApiKey", "fake-api-key"},
                {"OpenWeatherSettings:BaseUrl", "http://api.openweathermap.org/data/2.5/"},
                {"OpenWeatherSettings:CacheExpirationHours", "3"}
            })
            .Build();
    }

    [Fact]
    public async Task GetWeatherAsync_CacheHit_ReturnsCachedData()
    {
        var cachedWeather = new WeatherData { CityId = "123", JsonData = "cached-json" };

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("123", cachedWeather);

        var service = new WeatherService(
            memoryCache,
            _httpClient,
            _config,
            _loggerMock.Object!
        );

        var result = await service.GetWeatherAsync("123");

        Assert.Equal("cached-json", result.JsonData);
    }


    [Fact]
    public async Task GetWeatherAsync_CacheMiss_FetchesFromApi()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var service = new WeatherService(
            memoryCache,
            _httpClient,
            _config,
            _loggerMock.Object!
        );

        var result = await service.GetWeatherAsync("123");

        Assert.Contains("temp", result.JsonData);
        Assert.Equal("123", result.CityId);

        Assert.True(memoryCache.TryGetValue("123", out var cached));
        Assert.NotNull(cached);
    }

    [Fact]
    public async Task GetActualWeatherAsync_ReturnsLiveData()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var service = new WeatherService(
            memoryCache,
            _httpClient,
            _config,
            _loggerMock.Object!
        );

        var result = await service.GetActualWeatherAsync("456");

        Assert.NotNull(result);
        Assert.Equal("456", result.CityId);
        Assert.Contains("temp", result.JsonData);
    }
}
