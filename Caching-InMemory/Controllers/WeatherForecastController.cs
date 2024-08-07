using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Caching_InMemory.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ResponseCache(Duration = 20, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] { "latest" })]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;


        public WeatherForecastController(
            ILogger<WeatherForecastController> logger
            )
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public ActionResult<IEnumerable<WeatherForecast>> Get(bool latest = false)
        {

            _logger.LogInformation("Loading All Weather forecast...");

            return Ok(WeatherForecasts());
        }

        [HttpGet("{id:int}")]
        public ActionResult<WeatherForecast> GetOne(int id, bool latest = false)
        {

            _logger.LogInformation("Loading Single Weather forecast...");

            var match = WeatherForecasts().FirstOrDefault(f => f.Id == id);

            if (match == null) return NotFound();

            return Ok(match);
        }

        private static IEnumerable<WeatherForecast> WeatherForecasts()
        {

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Id = index,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });
        }


    }
}
