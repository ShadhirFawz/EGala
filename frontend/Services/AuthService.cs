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
                Console.WriteLine($"Sending login request to: {_http.BaseAddress}auth/login");
                Console.WriteLine($"Email: {model.Email}, Password length: {model.Password?.Length}");

                var response = await _http.PostAsJsonAsync("auth/login", model);

                Console.WriteLine($"Response status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {errorContent}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                Console.WriteLine($"Login response - Token: {!string.IsNullOrEmpty(result?.Token)}, Role: {result?.Role}");

                if (result?.Token == null)
                {
                    Console.WriteLine("No token in response");
                    return false;
                }

                await _localStorage.SetItemAsync("authToken", result.Token);
                Console.WriteLine("Token stored successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public string Name { get; set; } = "";
    }
}