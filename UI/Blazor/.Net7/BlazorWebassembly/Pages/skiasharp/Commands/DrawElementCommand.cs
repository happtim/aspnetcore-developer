using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class DrawElementCommand : ICommand
    {
        private DrawElement _element;
        public DrawElementCommand(DrawElement element)
        {
            _element = element;
        }

        public void Execute(DrawManager drawManager)
        {
            drawManager.AddElement(_element);
        }

        public void Undo(DrawManager drawManager)
        {
            drawManager.RemoveElement(_element);
        }
    }
}
