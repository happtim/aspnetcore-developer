using BlazorWebLLM.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<WebLLMService>();

await builder.Build().RunAsync();
