using BlazorWebassembly.Pages.skiasharp.Commands.Edits;
using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class EditElementCommand : ICommand
    {
        private DrawElement _element;
        private IEditOperation _operation;
        private object _oldState;
        private object _newState;

        public EditElementCommand(DrawElement element, IEditOperation operation, object oldState, object newState)
        {
            _element = element;
            _operation = operation;
            _oldState = oldState;
            _newState = newState;

        }

        public void Execute(DrawManager drawManager)
        {
            _operation.Apply(_element, _newState);
        }

        public void Undo(DrawManager drawManager)
        {
            _operation.Apply(_element, _oldState);
        }
    }
}
