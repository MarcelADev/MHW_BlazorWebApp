using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using MyMHWApp4;
using MyMHWApp4.Identity;
using MyMHWApp4.Provider;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// register the cookie handler
builder.Services.AddTransient<CookieHandler>();

// set up authorization
builder.Services.AddAuthorizationCore();

// register the custom state provider
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

// register the account management interface
builder.Services.AddScoped(
    sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

// set base address for default host
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.Configuration["FrontendUrl"] ?? "https://localhost:5002") });

// configure client for auth interactions
builder.Services.AddHttpClient(
    "Auth",
    opt => opt.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001"))
    .AddHttpMessageHandler<CookieHandler>();

#region SELFMADE SERVICES

builder.Services.AddScoped<HubConnection>(hc =>
    new HubConnectionBuilder()
        .WithUrl(builder.Configuration["BackendUrl"] + "/chathub")
        .WithAutomaticReconnect()
        .Build());

builder.Services.AddScoped<SessionProvider>();
builder.Services.AddScoped<PlayerProvider>();
builder.Services.AddScoped<ParticipatorProvider>();
builder.Services.AddScoped<InventoryEntryProvider>();
builder.Services.AddScoped<CategoryProvider>();
builder.Services.AddScoped<WeaponProvider>();
builder.Services.AddScoped<GameExtensionProvider>();

#endregion

await builder.Build().RunAsync();