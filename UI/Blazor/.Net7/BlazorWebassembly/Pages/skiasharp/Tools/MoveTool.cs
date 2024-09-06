using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class MoveTool : ITool
    {
        private DrawManager _drawManager;
        private ToolManager _toolManager;
        private DrawElement _hitElement;
        //位置差值
        private SKPoint _diff;
        private SKPoint _start;

        public MoveTool(DrawManager drawManager, ToolManager toolManager)
        {
            _drawManager = drawManager;
            _toolManager = toolManager;
        }

        public void MouseDown(SKPoint worldPoint)
        {
            _hitElement = _drawManager.HitTest(worldPoint);
            _start = _diff = worldPoint;
        }

        public void MouseMove(SKPoint worldPoint)
        {
            if (_hitElement == null) return;

            var dx = worldPoint.X - _diff.X;
            var dy = worldPoint.Y - _diff.Y;

            _hitElement.Move(dx, dy);

             _diff = worldPoint;

            _drawManager.Invalidate(); // 触发重绘
        }

        public void MouseUp(SKPoint worldPoint)
        {
            if (_hitElement != null)
            {
                var dx = worldPoint.X - _start.X;
                var dy = worldPoint.Y - _start.Y;

                _hitElement.Move(-dx, -dy);

                _drawManager.AddCommand(new MoveElementCommand(_hitElement, dx, dy));

                _hitElement = null;

                _toolManager.SetTool(null);
            }
        }
    }
}
