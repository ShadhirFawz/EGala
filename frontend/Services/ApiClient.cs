using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace frontend.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;

        public ApiClient(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        private async Task AddAuthHeaderAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("authToken");
            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            await AddAuthHeaderAsync();
            return await _http.GetFromJsonAsync<T>(url);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            await AddAuthHeaderAsync();
            return await _http.PostAsJsonAsync(url, data);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
        {
            await AddAuthHeaderAsync();
            return await _http.PutAsJsonAsync(url, data);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            await AddAuthHeaderAsync();
            return await _http.DeleteAsync(url);
        }
    }
}
