using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Caching_InMemory.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMemoryCache _memoryCache;
        private const string Cache1Key = "WeatherForecastCacheKey";
        private const string Cache2Key = "SecondCacheKey";


        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {

            if (_memoryCache.TryGetValue(Cache2Key, out IEnumerable<WeatherForecast>? weatherForecasts) && weatherForecasts != null)
            {
                _logger.LogInformation("WeatherForecast found in cache.");
            }

            else
            {
                _logger.LogInformation("WeatherForecast not found in cache. Fetching from database");


                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(1);

                weatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                }).ToArray();

                _memoryCache.Set(Cache1Key, weatherForecasts, cacheEntryOptions);
                _memoryCache.Set(Cache2Key, weatherForecasts, cacheEntryOptions);
            }

            return weatherForecasts;
        }
    }
}
