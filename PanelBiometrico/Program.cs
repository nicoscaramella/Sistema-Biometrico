using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PanelBiometrico;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Asegúrate de poner la URL real donde corre tu API (https o http)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5218") });
builder.Services.AddSingleton<PanelBiometrico.Services.AuthState>();
await builder.Build().RunAsync();
