using System.Net.Http.Json;
using VisitorInfoApi.Models;
using VisitorInfoApi.Models.DTOs;
using VisitorInfoApi.Services.Interfaces;

namespace VisitorInfoApi.Services
{
    public class VisitorInfoService : IVisitorInfoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public VisitorInfoService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<VisitorInfo> GetVisitorInfo(string ipAddress, string visitorName)
        {
            var ipStackResponse = await GetIpStackInfo(ipAddress);
            var weatherResponse = await GetWeatherInfo(ipStackResponse.Latitude, ipStackResponse.Longitude);

            return new VisitorInfo
            {
                ClientIp = ipAddress,
                Location = ipStackResponse.City,
                Greeting = $"Hello, {visitorName}! The temperature is {weatherResponse.Data.Values.Temperature:F1} degrees Celsius in {ipStackResponse.City}"
            };
        }

        private async Task<IpStackResponse> GetIpStackInfo(string ipAddress)
        {
            // var baseUrl = "http://api.ipstack.com/";
            // var apiKey = "e7e3eea930007034729837c9774b231a";

            // var url = $"{baseUrl}{ipAddress}?access_key={apiKey}";

            // var response = await _httpClient.GetFromJsonAsync<IpStackResponse>(url);

            // if (response == null || string.IsNullOrEmpty(response.City))
            // {
            //     throw new Exception("Unable to determine location from IP address");
            // }

            // return response;

            var url = $"https://api.ipgeolocation.io/ipgeo?apiKey=6e3e279ba70044d0ad992ea1ad1fe928&ip={ipAddress}&fields=city";

            var response = await _httpClient.GetFromJsonAsync<IpStackResponse>(url);

            if (response == null || string.IsNullOrEmpty(response.City))
            {
                throw new Exception("Unable to determine location from IP address");
            }

            return response;
        }

        private async Task<TomorrowioResponse> GetWeatherInfo(double latitude, double longitude)
        {
            var baseUrl = "https://api.tomorrow.io/v4/weather/realtime";

            var apiKey = "HtTW81xFvAVfr1ykZY5zoGRbZXVejLUL";

            var url = $"{baseUrl}?location={latitude},{longitude}&apikey={apiKey}";

            return await _httpClient.GetFromJsonAsync<TomorrowioResponse>(url);
        }
    }
}
