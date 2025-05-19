using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using WeatherApi.Models;

namespace WeatherApi.Services
{
    public interface IWeatherService
    {
        Task<WeatherData> GetWeatherAsync(string cityId);
        Task<WeatherData> GetActualWeatherAsync(string cityId);
    }
    public class WeatherService : IWeatherService
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherService> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrlWeather;
        private readonly int _cacheExpirationHours;

        public WeatherService(
            IMemoryCache cache,
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<WeatherService> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiKey = configuration["OpenWeatherSettings:ApiKey"]
                      ?? throw new ArgumentNullException("OpenWeatherSettings:ApiKey");

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                logger.LogError("OpenWeather API KEY is empty, check appsettings -> OpenWeatherSettings:ApiKey");
            }

            _baseUrlWeather = configuration["OpenWeatherSettings:BaseUrl"]
                              ?? throw new ArgumentNullException("OpenWeatherSettings:BaseUrl");
            _cacheExpirationHours = configuration.GetValue<int>("OpenWeatherSettings:CacheExpirationHours", 3);
        }

        public async Task<WeatherData> GetWeatherAsync(string cityId)
        {
            if (_cache.TryGetValue(cityId, out WeatherData? cachedData))
            {
                _logger.LogInformation($"[CACHE HIT] Returning cached weather data for cityId={cityId}");
                return cachedData ?? throw new InvalidOperationException("Cached data is null.");
            }

            _logger.LogInformation($"[CACHE MISS] Fetching fresh weather data for cityId={cityId}");

            var newData = await FetchWeatherDataFromApi(BuildWeatherApiUrl("forecast", new() { ["id"] = cityId }));

            var weatherData = new WeatherData
            {
                CityId = cityId,
                JsonData = newData,
            };

            _cache.Set(cityId, weatherData, TimeSpan.FromHours(_cacheExpirationHours));

            return weatherData;
        }


        public async Task<WeatherData> GetActualWeatherAsync(string cityId)
        {
            var data = await FetchWeatherDataFromApi(BuildWeatherApiUrl("weather", new() { ["id"] = cityId }));

            return new WeatherData
            {
                CityId = cityId,
                JsonData = data,
            };
        }

        private async Task<string> FetchWeatherDataFromApi(string urlWeather)
        {
            try
            {
                var response = await _httpClient.GetAsync(urlWeather);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning($"Not Found results using url: {urlWeather}");
                    return null;
                }

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather data from API");
                throw;
            }
        }
        private string BuildWeatherApiUrl(string endpoint, Dictionary<string, string> queryParams)
        {
            var baseUri = new Uri(new Uri(_baseUrlWeather), endpoint);
            if (queryParams is null)
            {
                queryParams = new Dictionary<string, string>()
                {
                    ["appid"] = _apiKey,
                    ["units"] = "metric",
                    ["lang"] = "pl"
                };
            }
            else
            {
                queryParams.Add("appid", _apiKey);
                queryParams.Add("units", "metric");
                queryParams.Add("lang", "pl");
            }

            return QueryHelpers.AddQueryString(baseUri.ToString(), queryParams!);
        }

    }
}
