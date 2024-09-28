using BlazorWebassembly.Pages.skiasharp.Commands.Edits;
using SkiaSharp;
using System.Reflection.Metadata.Ecma335;

namespace BlazorWebassembly.Pages.skiasharp.Draws
{
    public class TextElement : DrawElement
    {
        public SKPoint Position { get; set; }
        public string Text { get; set; }
        public SKPaint Paint { get; set; }
        public float Scale { get; set; } = 1.0f;
        public float BaseTextSize { get; set; }

        public float minScale = 0.2f;
        public float maxScale = 5.0f;

        public TextElement(SKPoint position, string text, SKColor color, SKTypeface typeface, float textSize = 16)
        {
            Scale = 1.0f;
            BaseTextSize = textSize;
            Position = position;
            Text = text;
            Paint = new SKPaint
            {
                Color = color,
                TextSize = BaseTextSize * Scale,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Typeface = typeface
            };
        }

        public override void Move(float dx, float dy)
        {
            Position = new SKPoint(Position.X + dx, Position.Y + dy);
        }

        public override void Draw(SKCanvas canvas)
        {
            canvas.DrawText(Text, Position, Paint);
        }

        public override void DrawHighlight(SKCanvas canvas)
        {
            var textBounds = GetHigtlightBounds();

            // 3. 绘制淡蓝色线框  
            using (var borderPaint = new SKPaint
            {
                Color = new SKColor(0, 0, 255, 120), // 淡蓝色，稍微调高透明度  
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1, // 线框宽度，可以根据需要调整  
                IsAntialias = true
            })
            {
                canvas.DrawRect(textBounds, borderPaint);
            }

            // 4. 绘制原始文字  
            canvas.DrawText(Text, Position, Paint);
        }

        public override bool IsHit(SKPoint point)
        {
            var textBounds = new SKRect();
            Paint.MeasureText(Text, ref textBounds);

            var hitRect = new SKRect(Position.X, Position.Y + textBounds.Top,
                Position.X + textBounds.Width, Position.Y + textBounds.Bottom);

            return hitRect.Contains(point);
        }

        public override bool IsContainedIn(SKRect rect)
        {
            var textBounds = new SKRect();
            Paint.MeasureText(Text, ref textBounds);
            var textRect = new SKRect(Position.X, Position.Y + textBounds.Top,
                Position.X + textBounds.Width, Position.Y + textBounds.Bottom);
            return rect.Contains(textRect);
        }

        public override void DrawControlPoints(SKCanvas canvas, int hoverControlPointIndex)
        {
            var controlPoints = GetControlPoints().ToList();
            using var paint = new SKPaint();


            for (int i = 0; i < controlPoints.Count; i++)
            {
                var point = controlPoints[i].Position;

                if (i == hoverControlPointIndex)
                {
                    paint.Style = SKPaintStyle.Fill;
                    paint.Color = new SKColor(0, 0, 255, 30);
                    canvas.DrawCircle(point, 10, paint);
                }

                paint.Style = SKPaintStyle.Fill;
                paint.Color = SKColors.White;
                canvas.DrawCircle(point, 5, paint);

                paint.Style = SKPaintStyle.Stroke;
                paint.Color = new SKColor(0, 0, 255, 120);
                paint.StrokeWidth = 1;
                canvas.DrawCircle(point, 5, paint);
            }
        }

        public override int GetControlPointIndex(SKPoint point)
        {
            var controlPoints = GetControlPoints().ToList();
            for (int i = 0; i < controlPoints.Count; i++)
            {
                if (SKPoint.Distance(point, controlPoints[i].Position) < 5)
                {
                    return i;
                }
            }
            return -1;
        }

        public IEnumerable<ControlPoint> GetControlPoints()
        {
            var bounds = GetHigtlightBounds();
            yield return new ControlPoint { Position = new SKPoint(bounds.Left, bounds.Top), Type = "LeftTop" };
            yield return new ControlPoint { Position = new SKPoint(bounds.Right, bounds.Top), Type = "RightTop" };
            yield return new ControlPoint { Position = new SKPoint(bounds.Right, bounds.Bottom), Type = "RightBottom" };
            yield return new ControlPoint { Position = new SKPoint(bounds.Left, bounds.Bottom), Type = "LeftBottom" };
            yield return new ControlPoint { Position = new SKPoint(bounds.MidX, bounds.Top), Type = "MidXTop" };
            yield return new ControlPoint { Position = new SKPoint(bounds.Right, bounds.MidY), Type = "RightMidY" };
            yield return new ControlPoint { Position = new SKPoint(bounds.MidX, bounds.Bottom), Type = "MidXBottom" };
            yield return new ControlPoint { Position = new SKPoint(bounds.Left, bounds.MidY), Type = "LeftMidY" };

        }

        private SKRect GetHigtlightBounds()
        {
            var textBounds = new SKRect();
            Paint.MeasureText(Text, ref textBounds);

            // 2. 扩展边界并移动到正确位置  
            textBounds = SKRect.Inflate(textBounds, 5, 5);
            textBounds.Offset(Position.X, Position.Y);

            return textBounds;
        }

        public SKRect GetTextBounds()
        {
            var textBounds = new SKRect();
            Paint.MeasureText(Text, ref textBounds);
            var textRect = new SKRect(Position.X, Position.Y + textBounds.Top,
                Position.X + textBounds.Width, Position.Y + textBounds.Bottom);

            return textRect;
        }

        public override void UpdateControlPoint(int index, SKPoint newPosition)
        {
            var bounds = GetTextBounds();
            var oldPosition = GetControlPoints().ElementAt(index).Position;
            float scaleFactor;

            switch (index)
            {
                case 0: // 左上角  
                    scaleFactor = SKPoint.Distance(newPosition, new SKPoint(bounds.Right, bounds.Bottom)) /
                                  SKPoint.Distance(oldPosition, new SKPoint(bounds.Right, bounds.Bottom));
                    ScaleText(scaleFactor, new SKPoint(bounds.Right, bounds.Bottom));
                    break;
                case 1: // 右上角  
                    scaleFactor = SKPoint.Distance(newPosition, new SKPoint(bounds.Left, bounds.Bottom)) /
                                  SKPoint.Distance(oldPosition, new SKPoint(bounds.Left, bounds.Bottom));
                    ScaleText(scaleFactor, new SKPoint(bounds.Left, bounds.Bottom));
                    break;
                case 2: // 右下角  
                    scaleFactor = SKPoint.Distance(newPosition, new SKPoint(bounds.Left, bounds.Top)) /
                                  SKPoint.Distance(oldPosition, new SKPoint(bounds.Left, bounds.Top));
                    ScaleText(scaleFactor, new SKPoint(bounds.Left, bounds.Top));
                    break;
                case 3: // 左下角  
                    scaleFactor = SKPoint.Distance(newPosition, new SKPoint(bounds.Right, bounds.Top)) /
                                  SKPoint.Distance(oldPosition, new SKPoint(bounds.Right, bounds.Top));
                    ScaleText(scaleFactor, new SKPoint(bounds.Right, bounds.Top));
                    break;
                case 4: // 上中  
                    scaleFactor = SKPoint.Distance(newPosition, new SKPoint(bounds.MidX, bounds.Bottom)) /
                                  SKPoint.Distance(oldPosition, new SKPoint(bounds.MidX, bounds.Bottom));
                    ScaleText(scaleFactor, new SKPoint(bounds.MidX, bounds.Bottom));
                    break;
                case 5: // 右中  
                    scaleFactor = SKPoint.Distance(newPosition, new SKPoint(bounds.Left, bounds.MidY)) /
                                  SKPoint.Distance(oldPosition, new SKPoint(bounds.Left, bounds.MidY));
                    ScaleText(scaleFactor, new SKPoint(bounds.Left, bounds.MidY));
                    break;
                case 6: // 下中  
                    scaleFactor = SKPoint.Distance(newPosition, new SKPoint(bounds.MidX, bounds.Top)) /
                                  SKPoint.Distance(oldPosition, new SKPoint(bounds.MidX, bounds.Top));
                    ScaleText(scaleFactor, new SKPoint(bounds.MidX, bounds.Top));
                    break;
                case 7: // 左中  
                    scaleFactor = SKPoint.Distance(newPosition, new SKPoint(bounds.Right, bounds.MidY)) /
                                  SKPoint.Distance(oldPosition, new SKPoint(bounds.Right, bounds.MidY));
                    ScaleText(scaleFactor, new SKPoint(bounds.Right, bounds.MidY));
                    break;
            }

         
        }

        private void ScaleText(float scaleFactor, SKPoint fixedPoint)
        {
            float newScale = Scale * scaleFactor;
            if (newScale <= minScale) { newScale = minScale; scaleFactor = 1.0f; }
            if (newScale >= maxScale) { newScale = maxScale; scaleFactor = 1.0f; }

            // 更新Scale属性  
            Scale = newScale;

            // 根据新的Scale更新Paint.TextSize  
            Paint.TextSize = BaseTextSize * Scale;

            // 调整位置以保持缩放基点不变  
            Position = new SKPoint(
                fixedPoint.X + (Position.X - fixedPoint.X) * scaleFactor,
                fixedPoint.Y + (Position.Y - fixedPoint.Y) * scaleFactor
            );
        }

        public override IEditOperation? GetEditOperation(int controlPointIndex)
        {
            var controlPoints = GetControlPoints().ToList();
            if (controlPointIndex >= 0 && controlPointIndex < controlPoints.Count)
            {
                return new TextScaleEditOperation(controlPoints[controlPointIndex].Type);
            }
            return null;
        }

        public override DrawElement Clone()
        {
            var clone = new TextElement(Position, Text, Paint.Color, Paint.Typeface, BaseTextSize);
            clone.Scale = this.Scale;
            clone.Paint.TextSize = clone.BaseTextSize * clone.Scale;
            return clone;
        }
    }
}
