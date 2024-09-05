using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class LineTool : ITool
    {
        private SKPoint _start;
        private SKPoint _end;
        private bool _isDrawing;
        private DrawContext _drawContext;

        public LineTool(DrawContext context)
        {
            _drawContext = context;
            _drawContext.CurrentTool = this;
        }

        public void MouseDown(float x, float y)
        {

            if (_isDrawing) 
            {
                //绘制结束 保存终点

                var screen = new SKPoint(x, y);

                _end = _drawContext.Viewport.ScreenToWorld(screen);

                _end = _drawContext.Viewport.OriginTransform(_end);

                _drawContext.UndoRedo.AddElement(new LineElement(_start, _end, SKColors.Black));

                _isDrawing = false;

                _drawContext.CurrentTool = null;

                _drawContext.PreviewElement = null;
            }
            else
            {
                //绘制开始 保存起点

                var screen = new SKPoint(x, y);

                _start = _drawContext.Viewport.ScreenToWorld(screen);

                _start = _drawContext.Viewport.OriginTransform(_start);

                _isDrawing = true;

            }

   
        }

        public void MouseMove(float x, float y)
        {
            if (_isDrawing)
            {
                var screen = new SKPoint(x, y);

                _end = _drawContext.Viewport.ScreenToWorld(screen);

                _end = _drawContext.Viewport.OriginTransform(_end);

                var line = new LineElement(_start, _end, SKColors.Black);

                _drawContext.PreviewElement = line;
                
                _drawContext.CanvasView.Invalidate(); // 触发重绘
            }
        }

        public void MouseUp(float x, float y)
        {

        }
    }
}
