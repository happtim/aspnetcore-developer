using System.Windows.Input;
using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class CommandManager
    {
        private DrawManager _drawManager;
        private readonly List<ICommand> _redoCommands = new List<ICommand>();
        private readonly List<ICommand> _commands = new List<ICommand>();

        public CommandManager(DrawManager drawManager)
        {

            _drawManager = drawManager;

        }

        public void AddCommand(ICommand command)
        {
            command.Execute(_drawManager);
            _commands.Add(command);
            _redoCommands.Clear();

            _drawManager.Invalidate(); // 触发重绘
        }

        public void Undo()
        {
            if (_commands.Count > 0)
            {
                var command = _commands[^1];
                command.Undo(_drawManager);

                _commands.RemoveAt(_commands.Count - 1);
                _redoCommands.Add(command);

                _drawManager.Invalidate(); // 触发重绘
            }
        }

        public void Redo()
        {
            if (_redoCommands.Count > 0)
            {
                var command = _redoCommands[^1];
                command.Execute(_drawManager);

                _redoCommands.RemoveAt(_redoCommands.Count - 1);
                _commands.Add(command);

                _drawManager.Invalidate(); // 触发重绘
            }
        }
    }
}
