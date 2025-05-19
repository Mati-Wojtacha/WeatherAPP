using Microsoft.EntityFrameworkCore;
using Moq;
using WeatherApi.Models;
using WeatherApi.Services;

public class CityServiceTests
{
    private readonly CityService _service;
    private readonly Mock<CityDbContext> _mockContext;
    private readonly DbContextOptions<CityDbContext> _dbOptions;

    public CityServiceTests()
    {
        _dbOptions = new DbContextOptionsBuilder<CityDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new CityDbContext(_dbOptions);

        // Przygotuj dane
        context.Cities.AddRange(
            new City { Id = "1", Name = "Warszawa", Country = "PL" },
            new City { Id = "2", Name = "Kraków", Country = "PL" },
            new City { Id = "3", Name = "Berlin", Country = "DE" }
        );
        context.SaveChanges();

        _service = new CityService(context);
    }

    [Fact]
    public async Task GetCitiesByCountry_ReturnsCorrectCities()
    {
        var result = await _service.GetCitiesByCountry("pl");

        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Equal("PL", c.Country));
    }

    [Fact]
    public async Task GetCityDetailsByName_ReturnsCorrectCity()
    {
        var result = await _service.GetCityDetailsByName("Kraków");

        Assert.NotNull(result);
        Assert.Equal("Kraków", result.Name);
    }

    [Fact]
    public async Task GetCitiesByCountryAndNameAsync_FiltersCorrectly()
    {
        var result = await _service.GetCitiesByCountryAndNameAsync("PL", "kra");

        Assert.Single(result);
        Assert.Equal("Kraków", result[0].Name);
    }

    [Fact]
    public async Task GetCitiesPaginated_ReturnsCorrectPage()
    {
        var result = await _service.GetCitiesPaginated("PL", 1, 1);

        Assert.Single(result.Items);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
    }
}
