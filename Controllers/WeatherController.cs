using Microsoft.AspNetCore.Mvc;
using WebApplicationWeather.Services;

namespace WebApplicationWeather.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : Controller
    {
        private readonly WeatherServices _weatherServices;
         public WeatherController(WeatherServices weatherServices)
        {
            _weatherServices = weatherServices;
        }
        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeather(string city)
        {
            try
            {
                var weather = await _weatherServices.GetWeatherAsync(city);
                return Ok(weather);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, "Error fetching weather data");
            }
        }
    }
}
