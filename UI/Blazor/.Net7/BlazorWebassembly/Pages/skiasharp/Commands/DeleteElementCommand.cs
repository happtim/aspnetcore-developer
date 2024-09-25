using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class DeleteElementCommand : ICommand
    {
        private readonly List<DrawElement> _elementsToDelete;
        private List<DrawElement> _deletedElements;

        public DeleteElementCommand(List<DrawElement> elementsToDelete)
        {
            _elementsToDelete = elementsToDelete;
        }

        public void Execute(DrawManager drawManager)
        {
            _deletedElements = new List<DrawElement>(_elementsToDelete);
            foreach (var element in _elementsToDelete)
            {
                drawManager.RemoveElement(element);
            }
        }

        public void Undo(DrawManager drawManager)
        {
            foreach (var element in _deletedElements)
            {
                drawManager.AddElement(element);
            }
        }
    }
}
