using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class PanTool : ITool
    {
        SKPoint? _touchLocation;
        DrawManager _drawManager;

        public PanTool(DrawManager drawManager)
        {
            _drawManager = drawManager;
            _drawManager.CurrentTool = this;
        }

        public void MouseDown(SKPoint worldPoint)
        {
            var screenPoint = _drawManager.Viewport.WorldToScreen(worldPoint);

            //screenPoint = _drawManager.Viewport.OriginTransform(screenPoint);

            _touchLocation = screenPoint;

        }

        public void MouseMove(SKPoint worldPoint)
        {
            if (_touchLocation == null) return;

            var screenPoint = _drawManager.Viewport.WorldToScreen(worldPoint);

            //screenPoint = _drawManager.Viewport.OriginTransform(screenPoint);

            float dx = screenPoint.X - _touchLocation.Value.X;
            float dy = screenPoint.Y - _touchLocation.Value.Y;

            _drawManager.Viewport.Pan(dx, dy);

            _touchLocation = screenPoint;

            _drawManager.Invalidate(); // 触发重绘
        }

        public void MouseUp(SKPoint worldPoint)
        {
            _touchLocation = null;

            _drawManager.CurrentTool = null;
        }
    }
}
