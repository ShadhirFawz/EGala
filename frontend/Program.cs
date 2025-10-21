using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using frontend;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
//using frontend.Services;
//using frontend.Auth;
//using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Base API URL (adjust if needed)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5000/api/";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Register services
// builder.Services.AddScoped<ApiClient>();
// builder.Services.AddScoped<AuthService>();
// builder.Services.AddScoped<EventService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
// builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

// builder.Services.AddMudServices(); // for UI styling (optional but nice)

// Run
await builder.Build().RunAsync();
