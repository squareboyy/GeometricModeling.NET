using SkiaSharp.Views.Blazor;
using SkiaSharp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.IO;
using GeometricModeling.NET.Client.Services.Interfaces;

namespace GeometricModeling.NET.Client.Pages
{
    public partial class Surface
    {
        private SKGLView skglView = null!;
        private SKPoint originPoint;
        private int width;
        private int height;
        private SKPoint3[]? axes3D;
        private int projection_angel = 25;

        [Inject]
        public required IAxesService Axes { get; set; }

        [Inject]
        public required IProjectionService Projection { get; set; }

        //draw some text
        //using var font = new SKFont
        //{
        //    Size = 24
        //};
        //canvas.DrawText($"SkiaSharp", coord, SKTextAlign.Center, font, paint);
        private void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            
            if (width != e.Info.Width || height != e.Info.Height)
            {
                width = e.Info.Width;
                height = e.Info.Height;
                ІnitializationAxes();
                Console.WriteLine($"W:{width} | H:{height}");
            }
         
            if (!originPoint.IsEmpty)
            {
                canvas.Translate(originPoint.X, height - originPoint.Y);
                canvas.Scale(1, -1);
            }
            canvas.Clear(SKColors.Black);

            using var paint = new SKPaint
            {
                Color = SKColors.Green,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 3
            };

            if (axes3D is not null)
            {
                var axes2D = Projection.GetDimetricProjection(axes3D, projection_angel);
                using var axesPath = Axes.GetAxesPath(axes2D);
                canvas.DrawPath(axesPath, paint);
            }
        }

        private void ІnitializationAxes()
        {
            originPoint = new SKPoint(width / 2, height / 2);
            axes3D = Axes.GetAxes3D(width, height, originPoint);
        }

        private void OnWheel(WheelEventArgs e)
        {
            if (e.DeltaY < 0 && projection_angel > -45)
                projection_angel--;
            else if (e.DeltaY > 0 && projection_angel < 45)
                projection_angel++;

            skglView.Invalidate();
        }
    }
}
