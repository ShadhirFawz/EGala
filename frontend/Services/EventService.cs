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
            _baseUrl = _config["ApiBaseUrl"] ?? "https://localhost:5000/api/";
        }

        public async Task<List<EventModel>> GetPublicEventsAsync(string? category = null, string? location = null, string? keyword = null)
        {
            var url = $"{_baseUrl}events/public";

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(category)) queryParams.Add($"category={Uri.EscapeDataString(category)}");
            if (!string.IsNullOrEmpty(location)) queryParams.Add($"location={Uri.EscapeDataString(location)}");
            if (!string.IsNullOrEmpty(keyword)) queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}");

            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            return await _http.GetFromJsonAsync<List<EventModel>>(url) ?? new List<EventModel>();
        }
    }
}
