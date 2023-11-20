using EasyMicroservices.UI.Places.Blazor.TestUI;
using EasyMicroservices.UI.Places.ViewModels.Cities;
using EasyMicroservices.UI.Places.ViewModels.Countries;
using EasyMicroservices.UI.Places.ViewModels.Provinces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Places.GeneratedServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new CityClient("http://localhost:2008", sp.GetService<HttpClient>()));
builder.Services.AddScoped(sp => new ProvinceClient("http://localhost:2008", sp.GetService<HttpClient>()));
builder.Services.AddScoped(sp => new CountryClient("http://localhost:2008", sp.GetService<HttpClient>()));

builder.Services.AddScoped<FilterCitiesListViewModel>();
builder.Services.AddScoped<AddOrUpdateCityViewModel>();

builder.Services.AddScoped<FilterProvincesListViewModel>();
builder.Services.AddScoped<AddOrUpdateProvinceViewModel>();

builder.Services.AddScoped<FilterCountriesListViewModel>();
builder.Services.AddScoped<AddOrUpdateCountryViewModel>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
});

await builder.Build().RunAsync();
