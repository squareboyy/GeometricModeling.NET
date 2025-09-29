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

            builder.Services.AddTransient<IAxesService, AxesService>();
            builder.Services.AddTransient<IProjectionService, ProjectionService>();
            builder.Services.AddTransient<IRotationTransformMatrixService, RotationTransformMatrixService>();
            builder.Services.AddTransient<IOrthographicProjectionMatrixService, OrthographicProjectionMatrixService>();

            await builder.Build().RunAsync();
        }
    }
}