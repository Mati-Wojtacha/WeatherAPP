using Microsoft.EntityFrameworkCore;
using WeatherApi.Models;
using WeatherApi.Responses;

namespace WeatherApi.Services
{
    public interface ICityService
    {
        Task<List<City>> GetCitiesByCountry(string countryCode);
        Task<City?> GetCityDetailsByName(string name);
        Task<List<City>> GetCitiesByCountryAndNameAsync(string countryCode, string nameFilter);
        Task<PaginatedResult<City>> GetCitiesPaginated(string countryCode, int pageNumber, int pageSize);
    }
    public class CityService : ICityService
    {
        private readonly CityDbContext _context;

        public CityService(CityDbContext context)
        {
            _context = context;
        }

        public async Task<List<City>> GetCitiesByCountry(string countryCode)
        {
            return await _context.Cities
                .Where(c => c.Country == countryCode.ToUpper())
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
        public async Task<City?> GetCityDetailsByName(string name)
        {
            return await _context.Cities
                .FirstOrDefaultAsync(c => EF.Functions.Like(c.Name, name));
        }

        public async Task<List<City>> GetCitiesByCountryAndNameAsync(string countryCode, string nameFilter)
        {
            return await _context.Cities
                .Where(c => c.Country == countryCode.ToUpper())
                .Where(c => EF.Functions.Like(c.Name, $"%{nameFilter}%"))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
        public async Task<PaginatedResult<City>> GetCitiesPaginated(string countryCode, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Cities
                .Where(c => c.Country == countryCode.ToUpper())
                .OrderBy(c => c.Name);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<City>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

    }
}