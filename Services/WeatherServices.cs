
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;


namespace WebApplicationWeather.Services
{
    public class WeatherServices
    {
        

         private readonly HttpClient _httpClient;
         private readonly IDatabase _cache;
         private readonly string _apiKey;

         public WeatherServices(HttpClient httpClient,IConnectionMultiplexer redis,IConfiguration configuration)
         {
            
         
                
            _httpClient = httpClient;
            _cache = redis.GetDatabase();
            _apiKey = configuration["ApiSettings:WeatherApiKey"];

        }
         public async Task<string> GetWeatherAsync(string city)
         {
             try
             {
                 string cacheKey = $"weather:{city}";
                 var cacheWeather = await _cache.StringGetAsync(cacheKey);
                 if (!cacheWeather.IsNullOrEmpty)
                 {
                     return cacheWeather;
                 }

                 var url = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{city}?include=fcst,obs,histfcst,stats,days,hours,current,alerts&key={_apiKey}&options=beta";

                 var response = await _httpClient.GetAsync(url);
                 response.EnsureSuccessStatusCode();
                 var weatherData = await response.Content.ReadAsStringAsync();

                 await _cache.StringSetAsync(cacheKey, weatherData, TimeSpan.FromHours(12));

                 return weatherData;
             }
             catch (HttpRequestException ex)
             {
                 Console.WriteLine($"Error fetching weather data: {ex.Message}");
                 return $"Error fetching weather data for {city}";
             }
         }


    }
}
