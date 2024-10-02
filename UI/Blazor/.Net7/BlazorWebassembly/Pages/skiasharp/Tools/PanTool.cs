using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class PanTool : ToolBase, ITool
    {
        SKPoint? _touchLocation;
        DrawingManager _drawManager;
        ToolManager _toolManager;
        ViewportManager _viewportManager;
        CursorManager _cursorManager;

        public PanTool(
            DrawingManager drawManager, 
            ToolManager toolManager , 
            ViewportManager viewportManager,
            CursorManager cursorManager
            )
        {
            _drawManager = drawManager;
            _toolManager = toolManager;
            _viewportManager = viewportManager;
            _cursorManager = cursorManager;
        }


        public async void MouseDown(SKPoint worldPoint)
        {
            var screenPoint = _viewportManager.WorldToScreen(worldPoint);

            _touchLocation = screenPoint;

            //TODO 设置抓手样式
            //await _cursorManager.SetHand();

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

        public async void MouseUp(SKPoint worldPoint)
        {
            _touchLocation = null;

            await _cursorManager.SetDefault();

        }
    }
}
