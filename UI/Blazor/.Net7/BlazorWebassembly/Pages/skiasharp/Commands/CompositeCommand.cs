using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class CompositeCommand : ICommand
    {
        private List<ICommand> _commands = new List<ICommand>();

        public void Add(ICommand command)
        {
            _commands.Add(command);
        }

        public void Execute(DrawingManager drawManager)
        {
            foreach (var command in _commands)
            {
                command.Execute(drawManager);
            }
        }

        public void Undo(DrawingManager drawManager)
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                _commands[i].Undo(drawManager);
            }
        }
    }
}
