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

        public override bool IsHit(SKPoint point)
        {
            // 简单的碰撞检测，可以根据需要改进  
            float threshold = 5;
            float d = PointToLineDistance(point, Start, End);
            return d <= threshold;
        }

        private float PointToLineDistance(SKPoint point, SKPoint lineStart, SKPoint lineEnd)
        {
            float x = point.X;
            float y = point.Y;
            float x1 = lineStart.X;
            float y1 = lineStart.Y;
            float x2 = lineEnd.X;
            float y2 = lineEnd.Y;

            float A = x - x1;
            float B = y - y1;
            float C = x2 - x1;
            float D = y2 - y1;

            float dot = A * C + B * D;
            float len_sq = C * C + D * D;
            float param = dot / len_sq;

            float xx, yy;

            if (param < 0)
            {
                xx = x1;
                yy = y1;
            }
            else if (param > 1)
            {
                xx = x2;
                yy = y2;
            }
            else
            {
                xx = x1 + param * C;
                yy = y1 + param * D;
            }

            float dx = x - xx;
            float dy = y - yy;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
