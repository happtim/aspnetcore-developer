using BlazorWebassembly.Pages.skiasharp.Commands;
using SkiaSharp.Views.Blazor;

namespace BlazorWebassembly.Pages.skiasharp
{
    public class DrawContext
    {
        public Viewport Viewport { get; set; }
        public SKCanvasView CanvasView { get; set; }
        public DrawElement? PreviewElement { get; set; }
        public ITool? CurrentTool { get; set; }
        public _9_UndoRedo UndoRedo { get; set; }
    }
}
