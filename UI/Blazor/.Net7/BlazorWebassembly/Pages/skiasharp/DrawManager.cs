using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Draws;
using BlazorWebassembly.Pages.skiasharp.Tools;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using System.Xml.Linq;

namespace BlazorWebassembly.Pages.skiasharp
{
    public class DrawManager
    {
        private readonly List<DrawElement> _elements = new List<DrawElement>();
        private readonly List<ICommand> _redoCommands = new List<ICommand>();
        private readonly List<ICommand> _commands = new List<ICommand>();
        private SKCanvasView _skiaView = null!;
        private Viewport _viewport = null!;
        public Viewport Viewport => _viewport;

        public DrawElement? PreviewElement { get; set; }
        public ITool? CurrentTool { get; set; }

        public DrawManager(SKCanvasView skiaView, Viewport viewport)
        {
            _skiaView = skiaView;
            _viewport = viewport;
        }

        public void AddCommand(ICommand command)
        {
            command.Execute(this);
            _commands.Add(command);
            _redoCommands.Clear();

            _skiaView.Invalidate(); // 触发重绘
        }

        public void AddElement(DrawElement element)
        {
            _elements.Add(element);
        }

        public void RemoveElement(DrawElement element)
        {
            _elements.Remove(element);
        }

        public void Draw(SKCanvas canvas)
        {
            foreach (var element in _elements)
            {
                element.Draw(canvas);
            }

            PreviewElement?.Draw(canvas);
        }

        public DrawElement HitTest(SKPoint point)
        {
            for (int i = _elements.Count - 1; i >= 0; i--)
            {
                if (_elements[i].IsHit(point))
                {
                    return _elements[i];
                }
            }
            return null;
        }

        public void Invalidate() 
        {
            _skiaView.Invalidate();
        }

        public void Undo()
        {
            if (_commands.Count > 0)
            {
                var command = _commands[^1];
                command.Undo(this);

                _commands.RemoveAt(_commands.Count - 1);
                _redoCommands.Add(command);

                _skiaView.Invalidate(); // 触发重绘
            }
        }

        public void Redo()
        {
            if (_redoCommands.Count > 0)
            {
                var command = _redoCommands[^1];
                command.Execute(this);

                _redoCommands.RemoveAt(_redoCommands.Count - 1);
                _commands.Add(command);

                _skiaView.Invalidate(); // 触发重绘
            }
        }
    }
}
