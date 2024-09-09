using SkiaSharp;
using System.Drawing;

namespace BlazorWebassembly.Pages.skiasharp
{
    public class ViewportManager
    {
        public enum OriginCoordinate
        {
            TopLeft,
            BottomLeft,
        }

        public SKSize ViewportSize { get; private set; }

        //底图大小
        public SKSize mapSize { get; private set; }

        //地图分辨率
        public float mapResolution = 0.05f;
        public float mapOriginOffsetX = 0f;
        public float mapOriginOffsetY = 0f;
        public float mapOriginOffsetTheta = 0f;


        public SKPoint Center => new SKPoint(ViewportSize.Width/dpiScale / 2, ViewportSize.Height / dpiScale / 2);

        public float dpiScale = 1.0f;

        public float _scale = 1.0f;

        public float scaleStep = 0.05f;

        public float minScale = 0.2f;
        public float maxScale = 5.0f;

        OriginCoordinate Origin { get; set; }

        float translateX = 0;
        float translateY = 0;

        public float Scale => dpiScale * _scale;

        public ViewportManager(SKSize viewportSize, 
            SKSize? mapSize = null,
            float mapResolution = 1f,
            float mapOriginOffsetX = 0f,
            float mapOriginOffsetY = 0f,
            float mapOriginOffsetTheta = 0f,
            float dpi = 1.0f,
            OriginCoordinate origin = OriginCoordinate.TopLeft)
        {
            this.mapSize = mapSize ?? new SKSize(viewportSize.Width, viewportSize.Height);
            Origin = origin;
            ViewportSize = viewportSize;
            _scale = 1.0f;
            dpiScale = dpi;
            maxScale = 5.0f / dpiScale;
            minScale = 0.2f / dpiScale;

            this.mapResolution = mapResolution;
            this.mapOriginOffsetX = mapOriginOffsetX;
            this.mapOriginOffsetY = mapOriginOffsetY;
            this.mapOriginOffsetTheta = mapOriginOffsetTheta;
        }

        public void Resize(SKSize viewportSize)
        {
            ViewportSize = viewportSize;
        }

        public void Zoom(float delta, SKPoint focus)
        {

            float oldScale = Scale;

            if (delta < 0)
            {
                // 放大
                var scale = this._scale * 1.1f;
                if (scale > this.maxScale)
                {
                    return;
                }

                this._scale = scale;
            }
            else
            {
                // 缩小
                var scale = this._scale / 1.1f;
                if (scale < this.minScale)
                {
                    return;
                }
                this._scale = scale;
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
            // 计算适合的缩放 & 计算考虑边距后的可用尺寸  
            float availableWidth = ViewportSize.Width / dpiScale - 2 * margin;
            float availableHeight = ViewportSize.Height / dpiScale - 2 * margin;

            // 计算适合的缩放
            float scaleX = availableWidth / contentBounds.Width;
            float scaleY = availableHeight / contentBounds.Height;

            _scale = Math.Min(scaleX, scaleY);

            // 确保缩放在允许的范围内  
            _scale = Math.Max(minScale, Math.Min(_scale, maxScale));

            // 计算居中的平移值  
            translateX = (ViewportSize.Width / dpiScale - contentBounds.Width * _scale) / 2 - contentBounds.Left * _scale;
            translateY = (ViewportSize.Height /dpiScale - contentBounds.Height * _scale) / 2 - contentBounds.Top * _scale;

        }

        public SKPoint WorldToScreen(SKPoint worldPoint)
        {
            return new SKPoint(worldPoint.X * _scale + translateX, worldPoint.Y * _scale + translateY);
        }

        public SKPoint ScreenToWorld(SKPoint screenPoint) 
        {
            return new SKPoint((screenPoint.X - translateX) / _scale, (screenPoint.Y - translateY) / _scale);
        }

        public SKPoint WorldToMap(SKPoint point) 
        {
            if (Origin == OriginCoordinate.TopLeft)
            {
                return new SKPoint((point.X + mapOriginOffsetX) / mapResolution, (point.Y + mapOriginOffsetY) / mapResolution);
            }
            else if (Origin == OriginCoordinate.BottomLeft) 
            {
                return new SKPoint( (point.X + mapOriginOffsetX) /mapResolution, mapSize.Height - (point.Y + mapOriginOffsetY) / mapResolution);
            }

            throw new ArgumentException("Unsupported  OriginCoordinate");
        }

        public SKPoint WorldFromMap(SKPoint point)
        {
            if (Origin == OriginCoordinate.TopLeft)
            {
                return new SKPoint(point.X * mapResolution - mapOriginOffsetX, point.Y * mapResolution - mapOriginOffsetY);
            }
            else if (Origin == OriginCoordinate.BottomLeft)
            {
                return new SKPoint(point.X * mapResolution - mapOriginOffsetX, (mapSize.Height - point.Y) * mapResolution - mapOriginOffsetY);
            }

            throw new ArgumentException("Unsupported  OriginCoordinate");
        }

        public SKMatrix GetTransformMatrix()
        {
            return SKMatrix.CreateScale(Scale, Scale)
                .PostConcat(SKMatrix.CreateTranslation(translateX * dpiScale, translateY * dpiScale));
        }
    }
}
