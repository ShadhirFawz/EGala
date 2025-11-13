using frontend.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;
using frontend.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace frontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthStateProvider _authStateProvider;

        public AuthService(HttpClient http, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _http = http;
            _localStorage = localStorage;
            _authStateProvider = (AuthStateProvider)authStateProvider;
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

                        // Notify authentication state change
                        _authStateProvider.NotifyUserAuthenticationStateChanged();
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
            _authStateProvider.NotifyUserAuthenticationStateChanged();
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public string Name { get; set; } = "";
    }
}