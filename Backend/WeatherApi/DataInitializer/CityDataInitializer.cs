using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO.Compression;
using WeatherApi.Models;

namespace WeatherApi.Data
{
    public class CityDataInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CityDataInitializer> _logger;
        private readonly IConfiguration _configuration;

        public CityDataInitializer(IServiceProvider serviceProvider, ILogger<CityDataInitializer> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CityDbContext>();

            try
            {
                if (await dbContext.Database.EnsureCreatedAsync(cancellationToken) || !await IsAnyCityDataAsync(dbContext))
                {
                    _logger.LogInformation("Importing data from file...");
                    await ImportDataAsync(dbContext);
                    _logger.LogInformation("Import finished successfully.");
                }
                else
                {
                    _logger.LogInformation("Data already exists in the database.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during data initialization.");
                throw;
            }
        }

        private async Task<bool> IsAnyCityDataAsync(CityDbContext context)
        {
            try
            {
                return await context.Cities.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking data.");
                return false;
            }
        }

        private async Task ImportDataAsync(CityDbContext context)
        {
            try
            {
                _logger.LogInformation("Starting data import...");

                var cities = await DownloadAndDecompressDataAsync();
                await SaveToDatabaseAsync(context, cities);

                _logger.LogInformation($"Imported {cities.Length} cities.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while importing data.");
                throw;
            }
        }

        private async Task<City[]> DownloadAndDecompressDataAsync()
        {
            using var httpClient = new HttpClient();

            _logger.LogInformation("Downloading file with cities...");
            var response = await httpClient.GetAsync(_configuration["OpenWeatherSettings:CitiesURL"]);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Decompressing data...");
            await using var gzipStream = new GZipStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress);
            using var reader = new StreamReader(gzipStream);
            var json = await reader.ReadToEndAsync();

            return JsonConvert.DeserializeObject<City[]>(json) ?? Array.Empty<City>();
        }

        private static async Task SaveToDatabaseAsync(CityDbContext context, City[] cities)
        {
            await context.Database.EnsureCreatedAsync();

            const int batchSize = 1000;
            for (int i = 0; i < cities.Length; i += batchSize)
            {
                var batch = cities.Skip(i).Take(batchSize);
                await context.Cities.AddRangeAsync(batch);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
            }
        }
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
