using SkiaSharp;
using System.Drawing;
using static System.Formats.Asn1.AsnWriter;

namespace BlazorWebassembly.Pages.skiasharp
{
    public class Viewport
    {
        public enum OriginCoordinate
        {
            TopLeft,
            BottomLeft,
        }

        public SKSize ViewportSize { get; private set; }

        public SKSize MapSize { get; private set; }

        public SKPoint Center => new SKPoint(ViewportSize.Width/DpiScale / 2, ViewportSize.Height / DpiScale / 2);

        public float DpiScale { get; private set; }

        public float _scale = 1.0f;

        public float scaleStep = 0.05f;

        public float minScale = 0.1f;
        public float maxScale = 5.0f;

        OriginCoordinate Origin { get; set; }

        float translateX = 0;
        float translateY = 0;

        public float Scale => DpiScale * _scale;

        public Viewport(SKSize viewportSize, SKSize? mapSize = null, float dpi = 1.0f,OriginCoordinate origin = OriginCoordinate.TopLeft)
        {
            MapSize = mapSize ?? new SKSize(viewportSize.Width, viewportSize.Height);
            Origin = origin;
            ViewportSize = viewportSize;
            _scale = 1.0f;
            DpiScale = dpi;
        }

        public void Zoom(float delta, SKPoint focus)
        {

            float oldScale = Scale;

            if (delta < 0)
            {
                // 放大
                this._scale = (float)Math.Round(this._scale + this.scaleStep , 2); // 解决小数点运算丢失精度的问题  
                if (this._scale > this.maxScale)
                {
                    this._scale = this.maxScale;
                    return;
                }
            }
            else
            {
                // 缩小
                this._scale = (float)Math.Round(this._scale - this.scaleStep, 2); // 解决小数点运算丢失精度的问题  
                if (this._scale < this.minScale)
                {
                    this._scale = this.minScale;
                    return;
                }
            }

            translateX = focus.X - (focus.X - translateX) * Scale / oldScale;
            translateY = focus.Y - (focus.Y - translateY) * Scale / oldScale;

        }

        public void Pan(float dx, float dy)
        {
            translateX += dx;
            translateY += dy;
        }

        public void ZoomToFit(SKRect contentBounds,float margin = 20f)
        {
            // 计算适合的缩放
            // 计算考虑边距后的可用尺寸  
            float availableWidth = ViewportSize.Width / DpiScale - 2 * margin;
            float availableHeight = ViewportSize.Height / DpiScale - 2 * margin;

            // 计算适合的缩放
            float scaleX = availableWidth / contentBounds.Width;
            float scaleY = availableHeight / contentBounds.Height;

            _scale = Math.Min(scaleX, scaleY);

            // 确保缩放在允许的范围内  
            _scale = Math.Max(minScale, Math.Min(_scale, maxScale));

            // 计算居中的平移值  
            translateX = (ViewportSize.Width / DpiScale - contentBounds.Width * _scale) / 2 - contentBounds.Left * _scale;
            translateY = (ViewportSize.Height /DpiScale - contentBounds.Height * _scale) / 2 - contentBounds.Top * _scale;

        }

        public SKPoint WorldToScreen(SKPoint worldPoint)
        {
            return new SKPoint(worldPoint.X * _scale + translateX, worldPoint.Y * _scale + translateY);
        }

        public SKPoint ScreenToWorld(SKPoint screenPoint) 
        {
            return new SKPoint((screenPoint.X - translateX) / _scale, (screenPoint.Y - translateY) / _scale);
        }

        public SKPoint OriginTransform(SKPoint point) 
        {
            if (Origin == OriginCoordinate.TopLeft)
            {
                return point;
            }
            else if (Origin == OriginCoordinate.BottomLeft) 
            {
                return new SKPoint(point.X, MapSize.Height - point.Y);
            }

            throw new ArgumentException("Unsupported  OriginCoordinate");

        }

        public SKMatrix GetTransformMatrix()
        {
            return SKMatrix.CreateScale(Scale, Scale)
                .PostConcat(SKMatrix.CreateTranslation(translateX * DpiScale, translateY * DpiScale));
        }
    }
}
