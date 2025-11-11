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
            var response = await _http.PostAsJsonAsync("api/auth/register", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Login(UserLogin model)
        {
            try
            {
                // FIX: Use the correct endpoint - your backend has [Route("api/[controller]")]
                var response = await _http.PostAsJsonAsync("auth/login", model);

                Console.WriteLine($"Login Response Status: {response.StatusCode}");
                Console.WriteLine($"Request URL: {_http.BaseAddress}auth/login");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (!string.IsNullOrEmpty(result?.Token))
                    {
                        await _localStorage.SetItemAsync("authToken", result.Token);
                        Console.WriteLine("✅ Login successful - Token stored");
                        return true;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Login failed: {response.StatusCode} - {errorContent}");
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Login exception: {ex.Message}");
                return false;
            }
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
        }

        public async Task<bool> TestConnection()
        {
            try
            {
                var response = await _http.GetAsync("auth/test");
                Console.WriteLine($"Connection test: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection test failed: {ex.Message}");
                return false;
            }
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public string Name { get; set; } = "";
    }
}