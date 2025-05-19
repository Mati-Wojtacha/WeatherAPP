using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherApi.Controllers;
using WeatherApi.Models;
using WeatherApi.Services;

public class WeatherControllerTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock = new();
    private readonly WeatherController _controller;

    public WeatherControllerTests()
    {
        _controller = new WeatherController(_weatherServiceMock.Object);
    }

    [Fact]
    public async Task GetWeather_EmptyCityId_ReturnsBadRequest()
    {
        var result = await _controller.GetWeather("");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("cityId is required.", badRequest.Value);
    }

    [Fact]
    public async Task GetWeather_ThrowsException_Returns503()
    {
        _weatherServiceMock.Setup(s => s.GetWeatherAsync("123"))
            .ThrowsAsync(new Exception("Service error"));

        var result = await _controller.GetWeather("123");

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(503, status.StatusCode);
        Assert.Equal("Service not available", status.Value);
    }

    [Fact]
    public async Task GetWeather_EmptyJsonData_ReturnsNotFound()
    {
        _weatherServiceMock.Setup(s => s.GetWeatherAsync("123"))
            .ReturnsAsync(new WeatherData { CityId = "123", JsonData = "" });

        var result = await _controller.GetWeather("123");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetWeather_ValidData_ReturnsContent()
    {
        var jsonData = "{\"temp\":25}";
        _weatherServiceMock.Setup(s => s.GetWeatherAsync("123"))
            .ReturnsAsync(new WeatherData { CityId = "123", JsonData = jsonData });

        var result = await _controller.GetWeather("123");

        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.Equal("application/json", contentResult.ContentType);
        Assert.Equal(jsonData, contentResult.Content);
    }

    [Fact]
    public async Task GetActualWeather_EmptyCityId_ReturnsBadRequest()
    {
        var result = await _controller.GetActualWeather("");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("cityId is required.", badRequest.Value);
    }

    [Fact]
    public async Task GetActualWeather_ThrowsException_Returns503()
    {
        _weatherServiceMock.Setup(s => s.GetActualWeatherAsync("456"))
            .ThrowsAsync(new Exception("Service error"));

        var result = await _controller.GetActualWeather("456");

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(503, status.StatusCode);
        Assert.Equal("Service not available", status.Value);
    }

    [Fact]
    public async Task GetActualWeather_EmptyJsonData_ReturnsNotFound()
    {
        _weatherServiceMock.Setup(s => s.GetActualWeatherAsync("456"))
            .ReturnsAsync(new WeatherData { CityId = "456", JsonData = "" });

        var result = await _controller.GetActualWeather("456");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetActualWeather_ValidData_ReturnsContent()
    {
        var jsonData = "{\"temp\":30}";
        _weatherServiceMock.Setup(s => s.GetActualWeatherAsync("456"))
            .ReturnsAsync(new WeatherData { CityId = "456", JsonData = jsonData });

        var result = await _controller.GetActualWeather("456");

        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.Equal("application/json", contentResult.ContentType);
        Assert.Equal(jsonData, contentResult.Content);
    }
}
