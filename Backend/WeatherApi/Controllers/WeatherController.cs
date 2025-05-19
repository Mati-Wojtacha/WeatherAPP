using Microsoft.AspNetCore.Mvc;
using WeatherApi.Services;

namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("api/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("{cityId}")]
        public async Task<IActionResult> GetWeather(string cityId)
        {
            if (string.IsNullOrWhiteSpace(cityId))
                return BadRequest("cityId is required.");

            try
            {
                var weatherData = await _weatherService.GetWeatherAsync(cityId);
                if (string.IsNullOrWhiteSpace(weatherData.JsonData))
                {
                    return NotFound();
                }

                return Content(weatherData.JsonData, "application/json");
            }
            catch
            {
                return StatusCode(503, "Service not available");
            }
        }

        [HttpGet("actual/{cityId}")]
        public async Task<IActionResult> GetActualWeather(string cityId)
        {
            if (string.IsNullOrWhiteSpace(cityId))
                return BadRequest("cityId is required.");

            try
            {
                var weatherData = await _weatherService.GetActualWeatherAsync(cityId);

                if (string.IsNullOrWhiteSpace(weatherData.JsonData))
                {
                    return NotFound();
                }
                return Content(weatherData.JsonData, "application/json");
            }
            catch
            {
                return StatusCode(503, "Service not available");
            }
        }
    }
}
