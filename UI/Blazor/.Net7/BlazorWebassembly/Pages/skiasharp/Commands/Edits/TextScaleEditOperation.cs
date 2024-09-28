using BlazorWebassembly.Pages.skiasharp.Draws;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Commands.Edits
{
    public class TextScaleEditOperation : IEditOperation
    {
        private string _type;
        public TextScaleEditOperation(string type)
        {
            _type = type;
        }
        public void Apply(DrawElement element, object newState, object oldState)
        {
            if (element is TextElement textElement && newState is SKPoint newPosition && oldState is SKPoint oldPosition)
            {
                var bounds = textElement.GetTextBounds();
                var fixedPoint = GetFixedPoint(bounds);

                float oldDistance = SKPoint.Distance(oldPosition, fixedPoint);
                float newDistance = SKPoint.Distance(newPosition, fixedPoint);
                float scaleFactor = newDistance / oldDistance;

                ApplyScale(textElement, scaleFactor, fixedPoint);
            }
        }

        private SKPoint GetFixedPoint(SKRect bounds)
        {
            switch (_type)
            {
                case "LeftTop": return new SKPoint(bounds.Right, bounds.Bottom);
                case "RightTop": return new SKPoint(bounds.Left, bounds.Bottom);
                case "RightBottom": return new SKPoint(bounds.Left, bounds.Top);
                case "LeftBottom": return new SKPoint(bounds.Right, bounds.Top);
                case "MidXTop": return new SKPoint(bounds.MidX, bounds.Bottom);
                case "RightMidY": return new SKPoint(bounds.Left, bounds.MidY);
                case "MidXBottom": return new SKPoint(bounds.MidX, bounds.Top);
                case "LeftMidY": return new SKPoint(bounds.Right, bounds.MidY);
                default: throw new ArgumentException("Invalid type");
            }
        }

        private void ApplyScale(TextElement textElement, float scaleFactor, SKPoint fixedPoint)
        {
            textElement.Scale *= scaleFactor;
            textElement.Paint.TextSize = textElement.BaseTextSize * textElement.Scale;

            textElement.Position = new SKPoint(
                fixedPoint.X + (textElement.Position.X - fixedPoint.X) * scaleFactor,
                fixedPoint.Y + (textElement.Position.Y - fixedPoint.Y) * scaleFactor
            );
        }

    }
}
