using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace frontend.Auth
{
    public class AuthStateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage = localStorage;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                Console.WriteLine($"AuthStateProvider - Token retrieved: {!string.IsNullOrEmpty(token)}");

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("No token found - returning anonymous");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                {
                    Console.WriteLine("Cannot read token - returning anonymous");
                    await _localStorage.RemoveItemAsync("authToken"); // Clear invalid token
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var jwtToken = handler.ReadJwtToken(token);
                Console.WriteLine($"Token subject: {jwtToken.Subject}, Expires: {jwtToken.ValidTo}");

                var claims = new List<Claim>();
                foreach (var claim in jwtToken.Claims)
                {
                    claims.Add(claim);
                    Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                }

                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);

                Console.WriteLine("Authentication state created successfully");
                return new AuthenticationState(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthStateProvider error: {ex.Message}");
                await _localStorage.RemoveItemAsync("authToken");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public void NotifyUserAuthenticationStateChanged()
        {
            Console.WriteLine("Notifying authentication state changed");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}