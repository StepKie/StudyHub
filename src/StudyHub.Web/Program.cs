using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using StudyHub.Core.Flashcards;
using StudyHub.Web;
using StudyHub.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddLocalization();
builder.Services.AddScoped<BrowserInterop>();
builder.Services.AddScoped<LanguageService>();
builder.Services.AddScoped<IDeckRepository, IndexedDbDeckRepository>();

var host = builder.Build();
LanguageService.ApplyStored((IJSInProcessRuntime)host.Services.GetRequiredService<IJSRuntime>());

await host.RunAsync();
