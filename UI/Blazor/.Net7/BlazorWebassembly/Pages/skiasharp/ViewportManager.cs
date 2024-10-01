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
        public SKSize MapSize { get; private set; }

        //地图分辨率
        public float _mapResolution = 0.05f;
        public float _mapOriginOffsetX = 0f;
        public float _mapOriginOffsetY = 0f;
        public float _mapOriginOffsetTheta = 0f;


        public SKPoint Center => new SKPoint(ViewportSize.Width/_dpiScale / 2, ViewportSize.Height / _dpiScale / 2);

        public float _dpiScale = 1.0f;

        public float _scale = 1.0f;

        public float _minScale = 0.2f;
        public float _maxScale = 5.0f;

        OriginCoordinate Origin { get; set; }

        float _translateX = 0;
        float _translateY = 0;

        public float Scale => _dpiScale * _scale;

        public ViewportManager(SKSize viewportSize, 
            SKSize? mapSize = null,
            float mapResolution = 1f,
            float mapOriginOffsetX = 0f,
            float mapOriginOffsetY = 0f,
            float mapOriginOffsetTheta = 0f,
            float dpi = 1.0f,
            OriginCoordinate origin = OriginCoordinate.TopLeft)
        {
            this.MapSize = mapSize ?? new SKSize(viewportSize.Width, viewportSize.Height);
            Origin = origin;
            ViewportSize = viewportSize;
            _scale = 1.0f;
            _dpiScale = dpi;
            _maxScale = 5.0f / _dpiScale;
            _minScale = 0.2f / _dpiScale;

            this._mapResolution = mapResolution;
            this._mapOriginOffsetX = mapOriginOffsetX;
            this._mapOriginOffsetY = mapOriginOffsetY;
            this._mapOriginOffsetTheta = mapOriginOffsetTheta;
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
                if (scale > this._maxScale)
                {
                    return;
                }

                this._scale = scale;
            }
            else
            {
                // 缩小
                var scale = this._scale / 1.1f;
                if (scale < this._minScale)
                {
                    return;
                }
                this._scale = scale;
            }

            _translateX = focus.X - (focus.X - _translateX) * Scale / oldScale;
            _translateY = focus.Y - (focus.Y - _translateY) * Scale / oldScale;

        }

        public void Pan(float dx, float dy)
        {
            _translateX += dx;
            _translateY += dy;
        }

        public void ZoomToFit(SKRect contentBounds,float margin = 20f)
        {
            // 计算适合的缩放 & 计算考虑边距后的可用尺寸  
            float availableWidth = ViewportSize.Width / _dpiScale - 2 * margin;
            float availableHeight = ViewportSize.Height / _dpiScale - 2 * margin;

            // 计算适合的缩放
            float scaleX = availableWidth / contentBounds.Width;
            float scaleY = availableHeight / contentBounds.Height;

            _scale = Math.Min(scaleX, scaleY);

            // 确保缩放在允许的范围内  
            _scale = Math.Max(_minScale, Math.Min(_scale, _maxScale));

            // 计算居中的平移值  
            _translateX = (ViewportSize.Width / _dpiScale - contentBounds.Width * _scale) / 2 - contentBounds.Left * _scale;
            _translateY = (ViewportSize.Height /_dpiScale - contentBounds.Height * _scale) / 2 - contentBounds.Top * _scale;

        }

        public SKPoint WorldToScreen(SKPoint worldPoint)
        {
            return new SKPoint(worldPoint.X * _scale + _translateX, worldPoint.Y * _scale + _translateY);
        }

        public SKPoint ScreenToWorld(SKPoint screenPoint) 
        {
            return new SKPoint((screenPoint.X - _translateX) / _scale, (screenPoint.Y - _translateY) / _scale);
        }

        public SKPoint WorldToMap(SKPoint point) 
        {
            if (Origin == OriginCoordinate.TopLeft)
            {
                return new SKPoint((point.X + _mapOriginOffsetX) / _mapResolution, (point.Y + _mapOriginOffsetY) / _mapResolution);
            }
            else if (Origin == OriginCoordinate.BottomLeft) 
            {
                return new SKPoint( (point.X + _mapOriginOffsetX) /_mapResolution, MapSize.Height - (point.Y + _mapOriginOffsetY) / _mapResolution);
            }

            throw new ArgumentException("Unsupported  OriginCoordinate");
        }

        public SKPoint WorldFromMap(SKPoint point)
        {
            if (Origin == OriginCoordinate.TopLeft)
            {
                return new SKPoint(point.X * _mapResolution - _mapOriginOffsetX, point.Y * _mapResolution - _mapOriginOffsetY);
            }
            else if (Origin == OriginCoordinate.BottomLeft)
            {
                return new SKPoint(point.X * _mapResolution - _mapOriginOffsetX, (MapSize.Height - point.Y) * _mapResolution - _mapOriginOffsetY);
            }

            throw new ArgumentException("Unsupported  OriginCoordinate");
        }

        public SKMatrix GetTransformMatrix()
        {
            return SKMatrix.CreateScale(Scale, Scale)
                .PostConcat(SKMatrix.CreateTranslation(_translateX * _dpiScale, _translateY * _dpiScale));
        }
    }
}
