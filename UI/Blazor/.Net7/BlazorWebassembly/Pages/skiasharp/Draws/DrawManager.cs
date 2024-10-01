using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Tools;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using System.Xml.Linq;

namespace BlazorWebassembly.Pages.skiasharp.Draws
{
    public class DrawManager
    {
        private readonly List<DrawingElement> _elements = new List<DrawingElement>();

        private SKCanvasView _skiaView = null!;
        private SelectedManager _selectedManager = null!;

        public DrawingElement? PreviewElement { get; set; }

        public DrawManager(SKCanvasView skiaView,SelectedManager selectedManager)
        {
            _skiaView = skiaView;
            _selectedManager = selectedManager;

            _selectedManager.SelectionChanged += (s, e) =>
            {
                skiaView.Invalidate();
            };
        }

        public void AddElement(DrawingElement element)
        {
            _elements.Add(element);
        }

        public void RemoveElement(DrawingElement element)
        {
            _elements.Remove(element);
        }

        public void Draw(SKCanvas canvas)
        {
            foreach (var element in _elements)
            {
                if (_selectedManager.IsEditMode(element)) 
                {
                    element.DrawHighlight(canvas);
                    element.DrawControlPoints(canvas, _selectedManager.GetHoverControlPointIndex());
                }
                else if (_selectedManager.Contains(element)) 
                {
                    element.DrawHighlight(canvas);
                }
                else if (_selectedManager.IsHover(element))
                {
                    element.DrawHighlight(canvas);
                }
                else
                {
                    element.Draw(canvas);
                }

            }

            PreviewElement?.Draw(canvas);
        }

        public DrawingElement HitTest(SKPoint point)
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


        public List<DrawingElement> GetElementsInRect(SKRect rect)
        {
            return _elements.Where(element => element.IsContainedIn(rect)).ToList();
        }


        public void Invalidate()
        {
            _skiaView.Invalidate();
        }


    }
}
