using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Refit;
using DynamicDataTest.Web.Data;
using DynamicDataTest.Web.Data.Profiles;
using DynamicDataTest.Web.Data.Trades;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services
        .AddRefitClient<IProfilesApiContract>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7231/api"));

builder.Services.AddScoped<IProfilesApiClient, ProfilesApiClient>();
builder.Services.AddScoped<IProfilesCache, ProfilesCache>();
builder.Services.AddScoped<ProfilesViewModel>();
builder.Services.AddScoped<TradeClient>();
builder.Services.AddScoped<TradeService>();
builder.Services.AddScoped<TradeViewModel>();
builder.Services.AddScoped<SearchHints>();
builder.Services.AddSingleton<TradeGenerator>();
builder.Services.AddSingleton<SchedulerProvider>();
builder.Services.AddSingleton<StaticData>();
builder.Services.AddSingleton<MarketDataService>();
builder.Services.AddLogging();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
