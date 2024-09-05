using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Tools
{
    public interface ITool
    {
        void MouseDown(SKPoint worldPoint);
        void MouseMove(SKPoint worldPoint);
        void MouseUp(SKPoint worldPoint);
    }
}
