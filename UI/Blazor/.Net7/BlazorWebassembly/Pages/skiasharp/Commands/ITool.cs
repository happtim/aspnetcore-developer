using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public interface ITool
    {
        void MouseDown(SKPoint worldPoint);
        void MouseMove(SKPoint worldPoint);
        void MouseUp(SKPoint worldPoint);
    }
}
