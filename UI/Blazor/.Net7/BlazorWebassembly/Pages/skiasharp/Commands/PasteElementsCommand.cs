using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class PasteElementsCommand : ICommand
    {
        private readonly List<DrawingElement> _pastedElements;

        public PasteElementsCommand(List<DrawingElement> pastedElements)
        {
            _pastedElements = pastedElements;
        }

        public void Execute(DrawingManager drawManager)
        {
            foreach (var element in _pastedElements)
            {
                drawManager.AddElement(element);
            }
        }

        public void Undo(DrawingManager drawManager)
        {
            foreach (var element in _pastedElements)
            {
                drawManager.RemoveElement(element);
            }
        }
    }
}
