using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class CopyPasteManager
    {
        private readonly SelectedManager _selectedManager;
        private readonly CommandManager _commandManager;
        private List<DrawingElement> _copiedElements = new List<DrawingElement>();

        public CopyPasteManager(
            SelectedManager selectedManager,
            CommandManager commandManager)
        {
            _selectedManager = selectedManager;
            _commandManager = commandManager;
        }

        public void Copy()
        {
            _copiedElements = _selectedManager.GetSelectedElements()
                .Select(e => e.Clone())
                .ToList();
        }

        public void Paste()
        {
            if (_copiedElements.Any())
            {

                // 对粘贴的元素进行偏移，以便它们不会完全重叠  
                OffsetElements(_copiedElements);

                var pasteCommand = new PasteElementsCommand(_copiedElements);
                _commandManager.AddCommand(pasteCommand);

                // 选中新粘贴的元素  
                _selectedManager.Clear();
            }
        }

        private void OffsetElements(List<DrawingElement> elements)
        {
            // 为粘贴的元素添加一个小偏移  
            const float offset = 10;
            foreach (var element in elements)
            {
                element.Move(offset, offset);
            }
        }
    }
}
