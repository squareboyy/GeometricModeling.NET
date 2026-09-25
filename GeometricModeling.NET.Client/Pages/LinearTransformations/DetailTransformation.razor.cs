using GeometricModeling.NET.Client.Services;
using GeometricModeling.NET.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Dynamic;
using System.Text.RegularExpressions;
using static System.MathF;

namespace GeometricModeling.NET.Client.Pages.LinearTransformations
{
    public partial class DetailTransformation : IDisposable
    {
        private const int SNAP_THRESHOLD = 15;
        private const int ROTATE_ANGLE = 12;

        private readonly SKPaint _paint = new()
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            IsAntialias = true
        };
        private readonly SKPaint _axesPaint = new()
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            IsAntialias = true
        };
        private readonly SKPaint _gridPaint = new()
        {
            Color = SKColors.Gray,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true
        };
        private readonly SKPaint _pathEffectPaint = new()
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            PathEffect = SKPathEffect.CreateDash([10, 5], 0),
            IsAntialias = true
        };
        private readonly SKPaint _dotPaint = new()
        {
            Color = SKColors.Black,
            StrokeWidth = 7,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true
        };
        private SKCanvasView _skiaView = null!;
        private bool _isClosedContour = false;
        private bool _isShiftWasPressed = false;
        private SKPath? _contourPath;
        private SKPoint _firstPoint;
        private SKPoint _possiblePoint;
        private SKPoint? _lastMousePos;

        [Inject]
        public ISurfaceStateService SurfaceStateService { get; set; } = null!;

        // Цей метод викличе Blazor, коли сторінка закриється
        public void Dispose()
        {
            _contourPath?.Dispose();
            _paint.Dispose();
            _axesPaint.Dispose();
            _gridPaint.Dispose();
            _pathEffectPaint.Dispose();
            _dotPaint.Dispose();
        }

        protected virtual void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var info = e.Info;

            if (info.Width != SurfaceStateService.Width || info.Height != SurfaceStateService.Height)
            {
                SurfaceStateService.Width = info.Width;
                SurfaceStateService.Height = info.Height;
                SurfaceStateService.GetAxesPoints();
                SurfaceStateService.GetGridPoints();
            }

            canvas.Translate(SurfaceStateService.OriginPoint);
            canvas.Clear(SKColors.White);

            if (SurfaceStateService.IsActiveGrid)
            {
                canvas.DrawPoints(SKPointMode.Lines, SurfaceStateService.GridPoints.ToArray(), _gridPaint);
            }

            if (SurfaceStateService.IsActiveAxes)
            {
                canvas.DrawPoints(SKPointMode.Lines, SurfaceStateService.AxedPoints.ToArray(), _axesPaint);
            }

            if (_contourPath is not null)
            {
                canvas.DrawPath(_contourPath, _paint);

                if (!_possiblePoint.IsEmpty)
                {
                    canvas.DrawLine(_contourPath.LastPoint, _possiblePoint, _pathEffectPaint);
                    canvas.DrawPoint(_possiblePoint, _dotPaint);
                }

                canvas.DrawPoints(SKPointMode.Points, _contourPath.Points, _dotPaint);
            }
        }

        private void OnPointerDown(PointerEventArgs e)
        {
            var touchLocation = new SKPoint((float)e.OffsetX, (float)e.OffsetY);
            touchLocation -= SurfaceStateService.OriginPoint;

            if (_contourPath is null || e.CtrlKey)
            {
                _isClosedContour = false;
                _contourPath = new SKPath(); //contourPath ??= new SKPath();
                _contourPath.MoveTo(touchLocation);
                _firstPoint = touchLocation;
                _lastMousePos = null;

                return;
            }

            /*if (e.ShiftKey)
            {
                _isClosedContour = false;
                _contourPath = new SKPath();

                float xGrid = Round(touchLocation.X / SurfaceStateService.GridStep) * SurfaceStateService.GridStep;
                float yGrid = Round(touchLocation.Y / SurfaceStateService.GridStep) * SurfaceStateService.GridStep;
                var point = new SKPoint(xGrid, yGrid);
                _contourPath.MoveTo(point);
                _firstPoint = point;

                _lastMousePos = null;

                return;
            }*/

            if (_isClosedContour && e.Buttons == 1)
            {
                _lastMousePos = touchLocation;

                return;
            }

            if (!_isClosedContour)
            {
                if (_possiblePoint == _firstPoint)
                {
                    _isClosedContour = true;
                    _contourPath.Close();
                    _possiblePoint = SKPoint.Empty;

                    return;
                }

                if (_isShiftWasPressed)
                {
                    _isShiftWasPressed = false;
                    _contourPath.LineTo(_possiblePoint);
                }
                else
                {
                    _contourPath.LineTo(touchLocation);
                }
            }
        }

        private void OnPointerMove(PointerEventArgs e)
        {
            if (_contourPath is null)
                return;

            var touchLocation = new SKPoint((float)e.OffsetX, (float)e.OffsetY);
            touchLocation -= SurfaceStateService.OriginPoint;

            if (_isClosedContour && e.Buttons == 1 && _lastMousePos is not null)
            {
                float dx = touchLocation.X - _lastMousePos.Value.X;
                float dy = touchLocation.Y - _lastMousePos.Value.Y;
                _contourPath.Transform(SKMatrix.CreateTranslation(dx, dy));

                _lastMousePos = touchLocation;

                return;
            }

            if (!_isClosedContour)
            {
                if (e.ShiftKey)
                {
                    _isShiftWasPressed = true;
                    float xGrid = Round(touchLocation.X / SurfaceStateService.GridStep) * SurfaceStateService.GridStep;
                    float yGrid = Round(touchLocation.Y / SurfaceStateService.GridStep) * SurfaceStateService.GridStep;
                    _possiblePoint = new SKPoint(xGrid, yGrid);

                    return;
                }

                if (CalcSnapDist(touchLocation, _firstPoint))
                {
                    _possiblePoint = _firstPoint;
                }
                else
                {
                    _possiblePoint = touchLocation;
                }
            }
        }

        private void OnWheel(WheelEventArgs e)
        {
            if (_contourPath is null || !_isClosedContour)
                return;

            var touchLocation = new SKPoint((float)e.OffsetX, (float)e.OffsetY);
            touchLocation -= SurfaceStateService.OriginPoint;
            int rotateAngle = 0;

            if (e.DeltaY < 0)
                rotateAngle = -ROTATE_ANGLE;
            else if (e.DeltaY > 0)
                rotateAngle = ROTATE_ANGLE;

            var rotationMatrix = SKMatrix.CreateRotationDegrees(rotateAngle, touchLocation.X, touchLocation.Y);
            _contourPath.Transform(rotationMatrix);
        }

        private void OnColorChanged(ChangeEventArgs e)
        {
            var newColor = e.Value?.ToString();

            if (string.IsNullOrEmpty(newColor))
                return;

            if (newColor.Equals("#ff0000"))
            {
                _pathEffectPaint.Color = SKColors.Green;
            }
            else
            {
                _pathEffectPaint.Color = SKColors.Red;
            }

            if (SKColor.TryParse(newColor, out var parsedColor))
            {
                _paint.Color = parsedColor;
            }
        }

        private void GetGridPoints()
        {
            SurfaceStateService.GetGridPoints();
        }

        private bool CalcSnapDist(SKPoint touchLocation, SKPoint snapPoint)
        {
            if (_contourPath is null)
                return false;

            var connectionDist = SKPoint.Distance(touchLocation, snapPoint);

            return _contourPath.Points.Length >= 3 && connectionDist <= SNAP_THRESHOLD;
        }
    }
}