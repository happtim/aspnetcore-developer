using BlazorWebassembly.Pages.skiasharp.Commands;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using System.Xml.Linq;

namespace BlazorWebassembly.Pages.skiasharp
{
    public class DrawManager
    {
        private readonly List<DrawElement> _elements = new List<DrawElement>();
        private readonly List<DrawElement> _redoElements = new List<DrawElement>();
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

        public void AddElement(DrawElement element)
        {
            _elements.Add(element);
            _redoElements.Clear();

            _skiaView.Invalidate(); // 触发重绘
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
            if (_elements.Count > 0)
            {
                var element = _elements[^1];
                _elements.RemoveAt(_elements.Count - 1);
                _redoElements.Add(element);

                _skiaView.Invalidate(); // 触发重绘
            }
        }

        public void Redo()
        {
            if (_redoElements.Count > 0)
            {
                var element = _redoElements[^1];
                _redoElements.RemoveAt(_redoElements.Count - 1);
                _elements.Add(element);

                _skiaView.Invalidate(); // 触发重绘
            }
        }
    }
}
