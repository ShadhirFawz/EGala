using System.Net.Http.Json;
using frontend.Models;

namespace frontend.Services
{
    public class EventService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly string _baseUrl;

        public EventService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
            _baseUrl = _config["ApiBaseUrl"] ?? "https://localhost:5203/api/";
        }

        public async Task<List<EventModel>> GetPublicEventsAsync(string? category = null, string? location = null, string? keyword = null)
        {
            try
            {
                // Clear auth header for public endpoint
                _http.DefaultRequestHeaders.Authorization = null;

                var url = $"{_baseUrl}events/public";

                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(category)) queryParams.Add($"category={Uri.EscapeDataString(category)}");
                if (!string.IsNullOrEmpty(location)) queryParams.Add($"location={Uri.EscapeDataString(location)}");
                if (!string.IsNullOrEmpty(keyword)) queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}");

                if (queryParams.Count > 0)
                    url += "?" + string.Join("&", queryParams);

                Console.WriteLine($"Fetching events from: {url}");

                var response = await _http.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var events = await response.Content.ReadFromJsonAsync<List<EventModel>>();
                    return events ?? new List<EventModel>();
                }
                else
                {
                    Console.WriteLine($"Error fetching events: {response.StatusCode}");
                    return new List<EventModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetPublicEventsAsync: {ex.Message}");
                return new List<EventModel>();
            }
        }

        public async Task<EventModel?> GetEventByIdAsync(string id)
        {
            try
            {
                // Clear auth header for public endpoint
                _http.DefaultRequestHeaders.Authorization = null;

                var url = $"{_baseUrl}events/{id}";
                Console.WriteLine($"Fetching event from: {url}");

                var response = await _http.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<EventModel>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Event not found: {id}");
                    return null;
                }
                else
                {
                    Console.WriteLine($"Error fetching event: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetEventByIdAsync: {ex.Message}");
                return null;
            }
        }
    }
}
