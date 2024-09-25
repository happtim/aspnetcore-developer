using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class PasteElementsCommand : ICommand
    {
        private readonly List<DrawElement> _pastedElements;

        public PasteElementsCommand(List<DrawElement> pastedElements)
        {
            _pastedElements = pastedElements;
        }

        public void Execute(DrawManager drawManager)
        {
            foreach (var element in _pastedElements)
            {
                drawManager.AddElement(element);
            }
        }

        public void Undo(DrawManager drawManager)
        {
            foreach (var element in _pastedElements)
            {
                drawManager.RemoveElement(element);
            }
        }
    }
}
