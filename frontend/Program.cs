using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using frontend.Services;
using frontend.Auth;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using frontend;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Base API URL (adjust if needed)
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("https://localhost:5203/api/") // Your backend URL
    });

// Register services
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EventService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

// FIX: Register both the implementation and the interface
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

builder.Services.AddMudServices();

// Run
await builder.Build().RunAsync();