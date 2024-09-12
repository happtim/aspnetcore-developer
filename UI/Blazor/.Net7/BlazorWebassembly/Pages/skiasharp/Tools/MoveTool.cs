using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class MoveTool : ITool
    {
        private DrawManager _drawManager;
        private ToolManager _toolManager;
        private CommandManager _commandManager;
        private List<DrawElement> _movedElements;
        //位置差值
        private SKPoint _diff;
        private SKPoint _start;

        public MoveTool(List<DrawElement> movedElements, DrawManager drawManager, ToolManager toolManager, CommandManager commandManager)
        {
            _drawManager = drawManager;
            _toolManager = toolManager;
            _commandManager = commandManager;
            _movedElements = movedElements;
        }

        public void MouseDown(SKPoint worldPoint)
        {
            _start = _diff = worldPoint;
        }

        public void MouseDrag(SKPoint worldPoint)
        {
            if (_movedElements == null || _movedElements.Count == 0) return;

            var dx = worldPoint.X - _diff.X;
            var dy = worldPoint.Y - _diff.Y;

            foreach (var movedElement in _movedElements)
            {
                movedElement.Move(dx, dy);
            }

            _diff = worldPoint;

            _drawManager.Invalidate(); // 触发重绘
        }

        public void MouseMove(SKPoint worldPoint)
        {

        }

        public void MouseUp(SKPoint worldPoint)
        {
            if (_movedElements != null && _movedElements.Count > 0)
            {
                var dx = worldPoint.X - _start.X;
                var dy = worldPoint.Y - _start.Y;

                foreach (var movedElement in _movedElements)
                {
                    movedElement.Move(-dx, -dy);
                }

                var compositeCommand = new CompositeCommand();

                foreach (var movedElement in _movedElements)
                {
                    compositeCommand.Add(new MoveElementCommand(movedElement, dx, dy));
                }

                _commandManager.AddCommand(compositeCommand);

                _movedElements.Clear();

                //_toolManager.SetTool(null);
            }
        }
    }
}
