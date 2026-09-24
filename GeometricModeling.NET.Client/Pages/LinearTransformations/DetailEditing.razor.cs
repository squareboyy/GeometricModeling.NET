using Microsoft.AspNetCore.Components.Web;
using SkiaSharp.Views.Blazor;

namespace GeometricModeling.NET.Client.Pages.LinearTransformations
{
    public partial class DetailEditing
    {
        private SKCanvasView _skiaView = null!;

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);
        }

        private void OnPointerDown(PointerEventArgs e)
        {
            // Handle pointer down event
        }

        private void OnPointerMove(PointerEventArgs e)
        {
            // Handle pointer move event
        }
    }
}
