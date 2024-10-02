using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class DrawElementCommand : ICommand
    {
        private DrawingElement _element;
        public DrawElementCommand(DrawingElement element)
        {
            _element = element;
        }

        public void Execute(DrawingManager drawManager)
        {
            drawManager.AddElement(_element);
        }

        public void Undo(DrawingManager drawManager)
        {
            drawManager.RemoveElement(_element);
        }
    }
}
