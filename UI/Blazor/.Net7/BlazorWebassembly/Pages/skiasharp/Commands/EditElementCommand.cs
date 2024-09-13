using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class EditElementCommand : ICommand
    {
        private DrawElement _element;
        public EditElementCommand(DrawElement element)
        {
            _element = element;
        }

        public void Execute(DrawManager drawManager)
        {
        }

        public void Undo(DrawManager drawManager)
        {
        }
    }
}
