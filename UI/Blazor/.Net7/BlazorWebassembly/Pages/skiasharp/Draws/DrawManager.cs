using BlazorWebassembly.Pages.skiasharp.Commands;
using BlazorWebassembly.Pages.skiasharp.Tools;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using System.Xml.Linq;

namespace BlazorWebassembly.Pages.skiasharp.Draws
{
    public class DrawManager
    {
        private readonly List<DrawElement> _elements = new List<DrawElement>();

        private SKCanvasView _skiaView = null!;
        private SelectedManager _selectedManager = null!;

        public DrawElement? PreviewElement { get; set; }

        public DrawManager(SKCanvasView skiaView,SelectedManager selectedManager)
        {
            _skiaView = skiaView;
            _selectedManager = selectedManager;
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
                //绘制选中效果
                if (_selectedManager.Contains(element)) 
                {
                    element.DrawHighlight(canvas);
                }
                else if (_selectedManager.HoverElement == element)
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


        public List<DrawElement> GetElementsInRect(SKRect rect)
        {
            return _elements.Where(element => element.IsContainedIn(rect)).ToList();
        }


        public void Invalidate()
        {
            _skiaView.Invalidate();
        }


    }
}
