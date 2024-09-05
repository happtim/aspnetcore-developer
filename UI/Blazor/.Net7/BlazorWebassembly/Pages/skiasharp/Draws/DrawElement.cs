using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Draws
{
    public abstract class DrawElement
    {
        public abstract void Draw(SKCanvas canvas);

        public abstract bool IsHit(SKPoint point);
    }
}
