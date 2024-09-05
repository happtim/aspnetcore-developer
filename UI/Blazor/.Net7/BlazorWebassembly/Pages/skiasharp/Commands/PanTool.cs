using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class PanTool : ITool
    {
        SKPoint? _touchLocation;
        DrawContext _drawContext;

        public PanTool(DrawContext context)
        {
            _drawContext = context;
            _drawContext.CurrentTool = this;
        }

        public void MouseDown(float x, float y)
        {
            _touchLocation = new SKPoint(x, y);
        }

        public void MouseMove(float x, float y)
        {
            if (_touchLocation == null) return;

            float dx = x - _touchLocation.Value.X;
            float dy = y - _touchLocation.Value.Y;

            _drawContext.Viewport.Pan(dx, dy);

            _touchLocation = new SKPoint(x, y);

            _drawContext.CanvasView.Invalidate(); // 触发重绘
        }

        public void MouseUp(float x, float y)
        {
            _touchLocation = null;

            _drawContext.CurrentTool = null;
        }
    }
}
