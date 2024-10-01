using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class DeleteElementCommand : ICommand
    {
        private readonly List<DrawingElement> _elementsToDelete;
        private List<DrawingElement> _deletedElements;

        public DeleteElementCommand(List<DrawingElement> elementsToDelete)
        {
            _elementsToDelete = elementsToDelete;
        }

        public void Execute(DrawManager drawManager)
        {
            _deletedElements = new List<DrawingElement>(_elementsToDelete);
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
