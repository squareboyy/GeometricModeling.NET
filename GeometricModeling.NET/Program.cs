//using GeometricModeling.NET.Client.Pages;
using GeometricModeling.NET.Client.Services.Interfaces;
using GeometricModeling.NET.Components;
using GeometricModeling.NET.Client.Services;

namespace GeometricModeling.NET
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            //зареєстровані в 2 збірках
            builder.Services.AddTransient<IAxesService, AxesService>();
            builder.Services.AddTransient<IProjectionService, ProjectionService>();
            builder.Services.AddTransient<IRotationTransformMatrixService, RotationTransformMatrixService>();
            builder.Services.AddTransient<IOrthographicProjectionMatrixService, OrthographicProjectionMatrixService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            app.Run();
        }
    }
}