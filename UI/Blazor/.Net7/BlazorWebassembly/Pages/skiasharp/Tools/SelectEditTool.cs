using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public class SelectEditTool : ITool
    {
        private DrawElement _drawElement;
        private int _controlPointIndex;
        private CommandManager _commandManager;
        private DrawManager _drawManager;
        public SelectEditTool(DrawElement drawElement,  CommandManager commandManager, DrawManager drawManager)
        {
            _drawElement = drawElement;
            _commandManager = commandManager;
            _drawManager = drawManager;
        }

        public void MouseDown(SKPoint worldPoint)
        {
            _controlPointIndex = _drawElement.GetControlPointIndex(worldPoint);
        }

        public void MouseMove(SKPoint worldPoint)
        {
        }

        public void MouseDrag(SKPoint worldPoint)
        {
            _drawElement.UpdateControlPoint(_controlPointIndex, worldPoint);
            _drawManager.Invalidate();
        }

        public void MouseUp(SKPoint worldPoint)
        {
            _commandManager.AddCommand(new EditElementCommand(_drawElement));
        }
    }
}
