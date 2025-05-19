using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WeatherApi.Services;

namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private readonly ICityService _importService;

        public CityController(ICityService importService)
        {
            _importService = importService;
        }

        [HttpGet("by-country/{countryCode}")]
        public async Task<IActionResult> GetCitiesByCountry(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
            {
                return BadRequest("Country code must be 2 characters long");
            }

            var cities = await _importService.GetCitiesByCountry(countryCode);

            if (!cities.Any())
            {
                return NotFound($"No cities found for country: {countryCode}");
            }

            return Ok(cities);
        }

        [HttpGet("by-country-and-name")]
        public async Task<IActionResult> GetCitiesByCountryAndName([FromQuery][Required] string countryCode, [FromQuery][Required] string nameFilter)
        {
            if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
            {
                return BadRequest("Country code must be 2 characters long");
            }

            var cities = await _importService.GetCitiesByCountryAndNameAsync(countryCode, nameFilter);

            if (!cities.Any())
            {
                return NotFound($"No cities found for country '{countryCode}' matching '{nameFilter}'");
            }

            return Ok(cities);
        }

        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetCityByName(string name)
        {
            var city = await _importService.GetCityDetailsByName(name);

            if (city is null)
            {
                return NotFound($"City not found: {name}");
            }

            return Ok(city);
        }

        [HttpGet("country/{countryCode}/paginated")]
        public async Task<IActionResult> GetByCountryPaginated(string countryCode, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _importService.GetCitiesPaginated(countryCode, page, pageSize);
            return Ok(result);
        }
    }

}