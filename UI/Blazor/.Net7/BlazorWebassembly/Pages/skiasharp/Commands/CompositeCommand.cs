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

        public void Execute(DrawManager drawManager)
        {
            foreach (var command in _commands)
            {
                command.Execute(drawManager);
            }
        }

        public void Undo(DrawManager drawManager)
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                _commands[i].Undo(drawManager);
            }
        }
    }
}
