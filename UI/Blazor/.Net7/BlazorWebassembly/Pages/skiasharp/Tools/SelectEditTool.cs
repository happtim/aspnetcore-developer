using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Commands.Edits;
using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class SelectEditTool : ToolBase, ITool
    {
        private DrawingElement _drawElement;
        private int _controlPointIndex;
        private CommandManager _commandManager;
        private DrawManager _drawManager;
        private SKPoint _start;
        private IEditOperation? _editOperation;

        public SelectEditTool(DrawingElement drawElement, CommandManager commandManager, DrawManager drawManager)
        {
            _drawElement = drawElement;
            _commandManager = commandManager;
            _drawManager = drawManager;
        }

        public void MouseDown(SKPoint worldPoint)
        {
            _start = worldPoint;
            _controlPointIndex = _drawElement.GetControlPointIndex(worldPoint);
            _editOperation = _drawElement.GetEditOperation(_controlPointIndex);

        }

        public void MouseDrag(SKPoint worldPoint)
        {
            _drawElement.UpdateControlPoint(_controlPointIndex, worldPoint);
            _drawManager.Invalidate();
        }

        public void MouseUp(SKPoint worldPoint)
        {
            _commandManager.AddCommand(
                new EditElementCommand(_drawElement, _editOperation, _start, worldPoint),
                execute:false);
        }
    }
}
