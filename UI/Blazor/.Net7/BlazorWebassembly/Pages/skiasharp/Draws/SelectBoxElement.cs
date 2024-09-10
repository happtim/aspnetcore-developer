using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Draws
{
    public class SelectBoxElement : DrawElement
    {
        public SKPoint Start { get; set; }
        public SKPoint End { get; set; }
        public SKPaint FillPaint { get; set; }
        public SKPaint StrokePaint { get; set; }

        public SelectBoxElement(SKPoint start, SKPoint end)
        {
            Start = start;
            End = end;

            // 填充画笔  
            FillPaint = new SKPaint
            {
                Color = new SKColor(0, 0, 255, 30), // 淡蓝色，带透明度  
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            // 边框画笔  
            StrokePaint = new SKPaint
            {
                Color = new SKColor(0, 0, 255, 100),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(new float[] { 5, 5 }, 0) // 虚线效果  
            };
        }

        public override void Draw(SKCanvas canvas)
        {
            // 3. 绘制边界框  
            var rect = SKRect.Create(
                Math.Min(Start.X, End.X),
                Math.Min(Start.Y, End.Y),
                Math.Abs(End.X - Start.X),
                Math.Abs(End.Y - Start.Y)
            );

            // 先绘制填充  
            canvas.DrawRect(rect, FillPaint);

            // 再绘制边框  
            canvas.DrawRect(rect, StrokePaint);
        }

        public override void DrawSelected(SKCanvas canvas)
        {

        }

        public override bool IsHit(SKPoint point)
        {
            var rect = SKRect.Create(
                Math.Min(Start.X, End.X),
                Math.Min(Start.Y, End.Y),
                Math.Abs(End.X - Start.X),
                Math.Abs(End.Y - Start.Y)
            );

            return rect.Contains(point);
        }

        public override bool IsContainedIn(SKRect rect)
        {
            return rect.Contains(Start) && rect.Contains(End);
        }

        public override void Move(float dx, float dy)
        {
            Start = new SKPoint(Start.X + dx, Start.Y + dy);
            End = new SKPoint(End.X + dx, End.Y + dy);
        }


    }
}
