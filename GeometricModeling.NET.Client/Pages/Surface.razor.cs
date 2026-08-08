using SkiaSharp.Views.Blazor;
using SkiaSharp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.IO;
using GeometricModeling.NET.Client.Services.Interfaces;
using GeometricModeling.NET.Client.Extensions;
using System.Collections.Generic;
using System.Diagnostics;

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

        //List<SKPoint[]>? surfaceProjectionPoints;

        [Inject]
        public required IAxesService Axes { get; set; }

        [Inject]
        public required IProjectionService Projection { get; set; }

        [Inject]
        public required ISurfaceService SurfaceService { get; set; }

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
                //Console.WriteLine($"W:{width} | H:{height}");
                //List<SKPoint3[]> surfacePoints = SurfaceService.GetProjectiveEllipsoid(200, 200, 200, 60, 360.GetRadianF(), 360.GetRadianF(), 0, 0);
                //surfaceProjectionPoints = surfacePoints.Select(p => Projection.GetDimetricProjection(p, projection_angel)).ToList();
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

            List<SKPoint3[]> surfacePoints = SurfaceService.GetProjectiveEllipsoid(200, 200, 200, 30, 360.GetRadianF(), 360.GetRadianF(), 0, 0);
            var timer = Stopwatch.StartNew();
            List<SKPoint[]> surfaceProjectionPoints = surfacePoints.Select(p => Projection.GetDimetricProjection(p, projection_angel)).ToList();
            timer.Stop();
            //var surfaceProjectionPoints = surfacePoints.Select(p => Projection.GetDimetricProjection(p, projection_angel));
            //surfaceProjectionPoints = surfacePoints.Select(p => Projection.GetDimetricProjection(p, projection_angel)).ToList();

            if (surfaceProjectionPoints is not null)
            {
                foreach (var projectionPoints in surfaceProjectionPoints)
                {
                    canvas.DrawPoints(SKPointMode.Polygon, projectionPoints, paint);
                }
            }
           
            Console.WriteLine($"time ms: {timer.ElapsedMilliseconds}");


            /*for (int i = 0; i < surfacePoints.Count; i++)
            {
                using var projectionPath = Projection.GetDimetricProjection2(surfacePoints[i], projection_angel);
                canvas.DrawPath(projectionPath, paint);
            }
            var timer = Stopwatch.StartNew();
            timer.Stop();
            Console.WriteLine($"time ms: {timer.ElapsedMilliseconds}");*/
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

           //skglView.Invalidate();
        }

        /*private async void OnWheel(WheelEventArgs e)
        {
            if (e.DeltaY < 0 && projection_angel > -45)
                while (projection_angel > -44)
                {
                    await (Task.Delay(10));
                    projection_angel--;
                }
            else if (e.DeltaY > 0 && projection_angel < 45)
                while (projection_angel < 44)
                {
                    await (Task.Delay(10));
                    projection_angel++;
                }
        }*/
    }
}
