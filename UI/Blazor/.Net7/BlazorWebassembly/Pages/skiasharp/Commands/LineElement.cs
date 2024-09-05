using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Commands
{
    public class LineElement : DrawElement
    {
        public SKPoint Start { get; set; }
        public SKPoint End { get; set; }
        public SKPaint Paint { get; set; }

        public LineElement(SKPoint start, SKPoint end, SKColor color, float strokeWidth = 2)
        {
            Start = start;
            End = end;
            Paint = new SKPaint
            {
                Color = color,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };
        }

        public override void Draw(SKCanvas canvas)
        {
            canvas.DrawLine(Start, End, Paint);
        }
    }
}
