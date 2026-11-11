using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using StudyHub.Core.Flashcards;
using StudyHub.Web;
using StudyHub.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddScoped<BrowserInterop>();
builder.Services.AddScoped<IDeckRepository, IndexedDbDeckRepository>();

await builder.Build().RunAsync();
