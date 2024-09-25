using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class SelectMoveTool : ToolBase, ITool
    {
        private DrawManager _drawManager;
        private CommandManager _commandManager;
        private SelectedManager _selectedManager;
        //位置差值
        private SKPoint _diff;
        private SKPoint _start;

        public SelectMoveTool(SelectedManager selectedManager, DrawManager drawManager, CommandManager commandManager)
        {
            _drawManager = drawManager;
            _selectedManager = selectedManager;
            _commandManager = commandManager;
        }

        public void MouseDown(SKPoint worldPoint)
        {
            _start = _diff = worldPoint;
        }

        public void MouseDrag(SKPoint worldPoint)
        {
            if (_selectedManager.Count() == 0) return;

            var dx = worldPoint.X - _diff.X;
            var dy = worldPoint.Y - _diff.Y;

            foreach (var movedElement in _selectedManager.GetSelectedElements())
            {
                movedElement.Move(dx, dy);
            }

            _diff = worldPoint;

            _drawManager.Invalidate(); // 触发重绘
        }

        public void MouseUp(SKPoint worldPoint)
        {
            if (_selectedManager.Count() > 0)
            {
                var dx = worldPoint.X - _start.X;
                var dy = worldPoint.Y - _start.Y;

                foreach (var movedElement in _selectedManager.GetSelectedElements())
                {
                    movedElement.Move(-dx, -dy);
                }

                var compositeCommand = new CompositeCommand();

                foreach (var movedElement in _selectedManager.GetSelectedElements())
                {
                    compositeCommand.Add(new MoveElementCommand(movedElement, dx, dy));
                }

                _commandManager.AddCommand(compositeCommand);

            }
        }
    }
}
