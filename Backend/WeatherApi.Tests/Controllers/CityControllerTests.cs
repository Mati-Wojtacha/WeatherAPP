using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherApi.Controllers;
using WeatherApi.Models;
using WeatherApi.Responses;
using WeatherApi.Services;

public class CityControllerTests
{
    private readonly Mock<ICityService> _cityServiceMock = new();
    private readonly CityController _controller;

    public CityControllerTests()
    {
        _controller = new CityController(_cityServiceMock.Object);
    }

    [Fact]
    public async Task GetCitiesByCountry_InvalidCountryCode_ReturnsBadRequest()
    {
        var result = await _controller.GetCitiesByCountry("P");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Country code must be 2 characters long", badRequest.Value);
    }

    [Fact]
    public async Task GetCitiesByCountry_NoCitiesFound_ReturnsNotFound()
    {
        _cityServiceMock.Setup(s => s.GetCitiesByCountry("PL"))
            .ReturnsAsync(new List<City>());

        var result = await _controller.GetCitiesByCountry("PL");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No cities found for country: PL", notFound.Value);
    }

    [Fact]
    public async Task GetCitiesByCountry_ReturnsOkWithCities()
    {
        var cities = new List<City> { new City { Id = "1", Name = "Warszawa" } };
        _cityServiceMock.Setup(s => s.GetCitiesByCountry("PL")).ReturnsAsync(cities);

        var result = await _controller.GetCitiesByCountry("PL");

        var ok = Assert.IsType<OkObjectResult>(result);
        var returnCities = Assert.IsAssignableFrom<IEnumerable<City>>(ok.Value);
        Assert.Single(returnCities);
    }

    [Fact]
    public async Task GetCitiesByCountryAndName_InvalidCountryCode_ReturnsBadRequest()
    {
        var result = await _controller.GetCitiesByCountryAndName("P", "Krak");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Country code must be 2 characters long", badRequest.Value);
    }

    [Fact]
    public async Task GetCitiesByCountryAndName_NoCitiesFound_ReturnsNotFound()
    {
        _cityServiceMock.Setup(s => s.GetCitiesByCountryAndNameAsync("PL", "Krak"))
            .ReturnsAsync(new List<City>());

        var result = await _controller.GetCitiesByCountryAndName("PL", "Krak");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No cities found for country 'PL' matching 'Krak'", notFound.Value);
    }

    [Fact]
    public async Task GetCitiesByCountryAndName_ReturnsOkWithCities()
    {
        var cities = new List<City> { new City { Id = "2", Name = "Kraków" } };
        _cityServiceMock.Setup(s => s.GetCitiesByCountryAndNameAsync("PL", "Krak"))
            .ReturnsAsync(cities);

        var result = await _controller.GetCitiesByCountryAndName("PL", "Krak");

        var ok = Assert.IsType<OkObjectResult>(result);
        var returnCities = Assert.IsAssignableFrom<IEnumerable<City>>(ok.Value);
        Assert.Single(returnCities);
    }

    [Fact]
    public async Task GetCityByName_CityNotFound_ReturnsNotFound()
    {
        _cityServiceMock.Setup(s => s.GetCityDetailsByName("NieznaneMiasto"))
            .ReturnsAsync((City?)null);

        var result = await _controller.GetCityByName("NieznaneMiasto");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("City not found: NieznaneMiasto", notFound.Value);
    }

    [Fact]
    public async Task GetCityByName_ReturnsOkWithCity()
    {
        var city = new City { Id = "3", Name = "Gdańsk" };
        _cityServiceMock.Setup(s => s.GetCityDetailsByName("Gdańsk"))
            .ReturnsAsync(city);

        var result = await _controller.GetCityByName("Gdańsk");

        var ok = Assert.IsType<OkObjectResult>(result);
        var returnCity = Assert.IsType<City>(ok.Value);
        Assert.Equal("Gdańsk", returnCity.Name);
    }

    [Fact]
    public async Task GetByCountryPaginated_ReturnsOkWithResult()
    {
        var pagedResult = new PaginatedResult<City>
        {
            Items = new List<City> { new City { Id = "4", Name = "Wrocław" } },
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 1
        };
        _cityServiceMock.Setup(s => s.GetCitiesPaginated("PL", 1, 10))
            .ReturnsAsync(pagedResult);

        var result = await _controller.GetByCountryPaginated("PL", 1, 10);

        var ok = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PaginatedResult<City>>(ok.Value);
        Assert.Single(returnValue.Items);
    }
}

