using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class PanTool : ITool
    {
        SKPoint? _touchLocation;
        DrawManager _drawManager;
        ToolManager _toolManager;
        ViewportManager _viewportManager;

        public PanTool(DrawManager drawManager, ToolManager toolManager , ViewportManager viewportManager)
        {
            _drawManager = drawManager;
            _toolManager = toolManager;
            _viewportManager = viewportManager;
        }

        public void MouseDown(SKPoint worldPoint)
        {
            var screenPoint = _viewportManager.WorldToScreen(worldPoint);

            //screenPoint = _drawManager.Viewport.OriginTransform(screenPoint);

            _touchLocation = screenPoint;

        }

        public void MouseMove(SKPoint worldPoint)
        {
            if (_touchLocation == null) return;

            var screenPoint = _viewportManager.WorldToScreen(worldPoint);

            //screenPoint = _drawManager.Viewport.OriginTransform(screenPoint);

            float dx = screenPoint.X - _touchLocation.Value.X;
            float dy = screenPoint.Y - _touchLocation.Value.Y;

            _viewportManager.Pan(dx, dy);

            _touchLocation = screenPoint;

            _drawManager.Invalidate(); // 触发重绘
        }

        public void MouseUp(SKPoint worldPoint)
        {
            _touchLocation = null;

            _toolManager.SetTool(null);
        }
    }
}
