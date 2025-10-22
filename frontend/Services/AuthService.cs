using frontend.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace frontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        public async Task<bool> Register(UserRegister model)
        {
            var response = await _http.PostAsJsonAsync("auth/register", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Login(UserLogin model)
        {
            var response = await _http.PostAsJsonAsync("auth/login", model);
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result?.Token == null) return false;

            await _localStorage.SetItemAsStringAsync("authToken", result.Token);
            return true;
        }

        public async Task Logout() => await _localStorage.RemoveItemAsync("authToken");
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
    }
}
