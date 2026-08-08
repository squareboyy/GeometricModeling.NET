using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Dynamic;
using static System.MathF;

namespace GeometricModeling.NET.Client.Pages
{
    public partial class LinearTransformations : IDisposable
    {
        private SKCanvasView skiaView = null!;

        private readonly SKPaint paint = new()
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            IsAntialias = true
        };

        private readonly SKPaint gridPaint = new()
        {
            Color = SKColors.Gray,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true
        };

        private readonly SKPaint pathEffectPaint = new()
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            PathEffect = SKPathEffect.CreateDash([10, 5], 0),
            IsAntialias = true
        };

        private readonly SKPaint dotPaint = new()
        {
            Color = SKColors.Black,
            StrokeWidth = 7,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true
        };

        private int width;
        private int height;

        private bool activeGrid = true;
        private int gridStep = 75;
        private List<SKPoint> gridPoints = [];
        
        private bool isClosedContour = false;
        private bool isShiftWasPressed = false;
        private SKPath? contourPath;
        private SKPoint firstPoint;
        private SKPoint possiblePoint;
        private SKPoint? lastMousePos;
        private const int SNAP_THRESHOLD = 15;
        private const int ROTATE_ANGLE = 12;

        private void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var info = e.Info;

            if (info.Width != width || info.Height != height)
            {
                width = info.Width;
                height = info.Height;
                GetGrid();
            }
            
            canvas.Clear(SKColors.White);

            if (activeGrid)
            {
                canvas.DrawPoints(SKPointMode.Lines, gridPoints.ToArray(), gridPaint);
            }

            if (contourPath is not null)
            {
                canvas.DrawPath(contourPath, paint);

                if (!possiblePoint.IsEmpty)
                {
                    canvas.DrawLine(contourPath.LastPoint, possiblePoint, pathEffectPaint);
                    canvas.DrawPoint(possiblePoint, dotPaint);
                }
                
                canvas.DrawPoints(SKPointMode.Points, contourPath.Points, dotPaint);
            }
        }

        private void OnPointerDown(PointerEventArgs e)
        {
            var touchLocation = new SKPoint((float)e.OffsetX, (float)e.OffsetY);

            if (contourPath is null || e.CtrlKey)
            {
                isClosedContour = false;
                contourPath = new SKPath(); //contourPath ??= new SKPath();
                contourPath.MoveTo(touchLocation);
                firstPoint = touchLocation;
                lastMousePos = null;

                return;
            }

            /*if (e.ShiftKey)
            {
                isClosedContour = false;
                contourPath = new SKPath();

                float xGrid = Round(touchLocation.X / gridStep) * gridStep;
                float yGrid = Round(touchLocation.Y / gridStep) * gridStep;
                var point = new SKPoint(xGrid, yGrid);
                contourPath.MoveTo(point);
                firstPoint = point;

                lastMousePos = null;

                return;
            }*/

            if (isClosedContour && e.Buttons == 1)
            {
                lastMousePos = touchLocation;

                return;
            }

            if (!isClosedContour)
            {
                if (possiblePoint == firstPoint)
                {
                    isClosedContour = true;
                    contourPath.Close();
                    possiblePoint = SKPoint.Empty;

                    return;
                }

                if (isShiftWasPressed)
                {
                    isShiftWasPressed = false;
                    contourPath.LineTo(possiblePoint);
                }
                else
                {
                    contourPath.LineTo(touchLocation);
                }
            }
        }

        private void OnPointerMove(PointerEventArgs e)
        {
            if (contourPath is null)
                return;

            var touchLocation = new SKPoint((float)e.OffsetX, (float)e.OffsetY);

            if (isClosedContour && e.Buttons == 1 && lastMousePos is not null)
            {
                float dx = touchLocation.X - lastMousePos.Value.X;
                float dy = touchLocation.Y - lastMousePos.Value.Y;
                contourPath.Transform(SKMatrix.CreateTranslation(dx, dy));

                lastMousePos = touchLocation;

                return;
            }

            if (!isClosedContour)
            {
                if (e.ShiftKey)
                {
                    isShiftWasPressed = true;
                    float xGrid = Round(touchLocation.X / gridStep) * gridStep;
                    float yGrid = Round(touchLocation.Y / gridStep) * gridStep;
                    possiblePoint = new SKPoint(xGrid, yGrid);

                    return;
                }

                if (CalcSnapDist(touchLocation, firstPoint))
                {
                    possiblePoint = firstPoint;
                }
                else
                {
                    possiblePoint = touchLocation;
                }
            }
        }

        private void OnWheel(WheelEventArgs e)
        {
            if (contourPath is null || !isClosedContour)
                return;

            var touchLocation = new SKPoint((float)e.OffsetX, (float)e.OffsetY);
            int rotateAngle = 0;

            if (e.DeltaY < 0)
                rotateAngle = -ROTATE_ANGLE;
            else if (e.DeltaY > 0)
                rotateAngle = ROTATE_ANGLE;

            var rotationMatrix = SKMatrix.CreateRotationDegrees(rotateAngle, touchLocation.X, touchLocation.Y);
            contourPath.Transform(rotationMatrix);
        }

        private void OnColorChanged(ChangeEventArgs e)
        {
            var newColor = e.Value?.ToString();

            if (string.IsNullOrEmpty(newColor))
                return;

            if (newColor.Equals("#ff0000"))
            {
                pathEffectPaint.Color = SKColors.Green;
            }
            else
            {
                pathEffectPaint.Color = SKColors.Red;
            }

            if (SKColor.TryParse(newColor, out var parsedColor))
            {
                paint.Color = parsedColor;
            }
        }

        private void GetGrid() //TODO: move to service(єдиний сервіс, що пов'язаний з елементами поверхні)
        {
            gridPoints.Clear();
            
            for (int x = 0; x <= width; x += gridStep)
            {
                gridPoints.Add(new SKPoint(x, 0));
                gridPoints.Add(new SKPoint(x, height));
            }

            for (int y = 0; y <= height; y += gridStep)
            {
                gridPoints.Add(new SKPoint(0, y));
                gridPoints.Add(new SKPoint(width, y));
            }
        }

        private bool CalcSnapDist(SKPoint touchLocation, SKPoint snapPoint)
        {
            if (contourPath is null)
                return false;

            var connectionDist = SKPoint.Distance(touchLocation, snapPoint);

            return contourPath.Points.Length >= 3 && connectionDist <= SNAP_THRESHOLD;
        }

        // Цей метод викличе Blazor, коли сторінка закриється
        public void Dispose()
        {
            contourPath?.Dispose();
            paint.Dispose();
            gridPaint.Dispose();
            pathEffectPaint.Dispose();
            dotPaint.Dispose();
        }
    }
}
