using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class LineTool : ITool
    {
        private SKPoint _start;
        private SKPoint _end;
        private bool _isDrawing;
        private DrawManager _drawManager;

        public LineTool(DrawManager drawManager)
        {
            _drawManager = drawManager;
            _drawManager.CurrentTool = this;
        }

        public void MouseDown(SKPoint worldPoint)
        {

            if (_isDrawing) 
            {
                //绘制结束 保存终点
                _end = worldPoint;

                _drawManager.AddElement(new LineElement(_start, _end, SKColors.Black));

                _isDrawing = false;

                _drawManager.CurrentTool = null;

                _drawManager.PreviewElement = null;
            }
            else
            {
                //绘制开始 保存起点

                _start = worldPoint;

                _isDrawing = true;

            }

   
        }


        public void MouseMove(SKPoint worldPoint)
        {
            if (_isDrawing)
            {
                _end = worldPoint;

                var line = new LineElement(_start, _end, SKColors.Black);

                _drawManager.PreviewElement = line;

                _drawManager.Invalidate(); // 触发重绘
            }
        }

        public void MouseUp(SKPoint worldPoint)
        {

        }
    }
}
