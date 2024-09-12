using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Draws
{
    public abstract class DrawElement
    {
        public abstract void Draw(SKCanvas canvas);
        public abstract void DrawHighlight(SKCanvas canvas);

        public abstract bool IsHit(SKPoint point);
        public abstract bool IsContainedIn(SKRect rect);

        public abstract void Move(float dx, float dy);

        public abstract void DrawEditMode(SKCanvas canvas);
        public abstract void DrawControlPoints(SKCanvas canvas);
        public abstract int GetControlPointIndex(SKPoint point);
        public abstract void UpdateControlPoint(int index, SKPoint newPosition);
    }
}
