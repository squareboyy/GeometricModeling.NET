using GeometricModeling.NET.Client.Services;
using GeometricModeling.NET.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace GeometricModeling.NET.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddScoped<ISurfaceStateService, SurfaceStateService>();

            await builder.Build().RunAsync();
        }
    }
}