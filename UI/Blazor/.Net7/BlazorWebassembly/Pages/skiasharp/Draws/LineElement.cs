using BlazorWebassembly.Pages.skiasharp.Commands.Edits;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Draws
{
    public class LineElement : DrawingElement
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

        public override void Move(float dx, float dy)
        {
            Start = new SKPoint(Start.X + dx, Start.Y + dy);
            End = new SKPoint(End.X + dx, End.Y + dy);
        }

        public override void Draw(SKCanvas canvas)
        {
            canvas.DrawLine(Start, End, Paint);
        }

        public override void DrawHighlight(SKCanvas canvas)
        {
            // 1. 绘制粗一些的线条作为背景  
            using (var paint = new SKPaint
            {
                Color = SKColors.LightBlue,
                StrokeWidth = Paint.StrokeWidth + 4,
                StrokeCap =  SKStrokeCap.Round,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            })
            {
                canvas.DrawLine(Start, End, paint);
            }

            // 2. 绘制原始线条  
            canvas.DrawLine(Start, End, Paint);

        }

        public override bool IsHit(SKPoint point)
        {
            // 简单的碰撞检测，可以根据需要改进  
            float threshold = 5;
            float d = PointToLineDistance(point, Start, End);
            return d <= threshold;
        }

        public override bool IsContainedIn(SKRect rect)
        {
            return rect.Contains(Start) && rect.Contains(End);
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

        public override void DrawControlPoints(SKCanvas canvas, int hoverControlPointIndex)
        {

            using var paint = new SKPaint();

            var index = 0;
            foreach (var point in GetControlPoints())
            {

                if (index == hoverControlPointIndex)
                {
                    paint.Style = SKPaintStyle.Fill;
                    paint.Color = new SKColor(0, 0, 255, 30);
                    canvas.DrawCircle(point.Position, 10, paint);
                }

                paint.Style = SKPaintStyle.Fill;
                paint.Color = SKColors.White;

                canvas.DrawCircle(point.Position, 5, paint);

                paint.Style = SKPaintStyle.Stroke;
                paint.Color = new SKColor(0, 0, 255, 120);
                paint.StrokeWidth = 1;

                canvas.DrawCircle(point.Position, 5, paint);

               index++;
                
            }
        }

        public override int GetControlPointIndex(SKPoint point)
        {
            if (SKPoint.Distance(point,Start) < 5) return 0;
            if (SKPoint.Distance(point, End) < 5) return 1;
            return -1;
        }


        public IEnumerable<ControlPoint> GetControlPoints()
        {
            yield return new ControlPoint { Position = Start, Type = "Start" };
            yield return new ControlPoint { Position = End, Type = "End" };
        }

        public override void UpdateControlPoint(int index, SKPoint newPosition)
        {
            if (index == 0) Start = newPosition;
            else if (index == 1) End = newPosition;
        }

        public override IEditOperation? GetEditOperation(int controlPointIndex)
        {
            if (controlPointIndex == 0) return new LineStartPointEditOperation();
            if (controlPointIndex == 1) return new LineEndPointEditOperation();
            return null;
        }

        public override DrawingElement Clone()
        {
            return new LineElement(Start, End, Paint.Color, Paint.StrokeWidth);
        }
    }
}
