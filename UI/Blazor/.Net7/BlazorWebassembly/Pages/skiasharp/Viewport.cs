using SkiaSharp;
using static System.Formats.Asn1.AsnWriter;

namespace BlazorWebassembly.Pages.skiasharp
{
    public class Viewport
    {
        public float DpiScale { get; private set; }

        public float _scale = 1.0f;

        public float scaleStep = 0.05f;

        public float minScale = 0.1f;
        public float maxScale = 5.0f;

        float translateX = 0;
        float translateY = 0;

        public float Scale => DpiScale * _scale;

        public Viewport(SKSize size,float dpi = 1.0f)
        {
            _scale = 1.0f;
            DpiScale = dpi;
        }

        public void Zoom(float delta, SKPoint focus)
        {

            float oldScale = Scale;

            if (delta < 0)
            {
                // 缩小  
                this._scale = (float)Math.Round(this.scaleStep + this._scale, 2); // 解决小数点运算丢失精度的问题  
                if (this._scale > this.maxScale)
                {
                    this._scale = this.maxScale;
                    return;
                }
            }
            else
            {
                // 放大  
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

        public SKPoint WorldToScreen(SKPoint worldPoint)
        {
            return new SKPoint(worldPoint.X * Scale + translateX, worldPoint.Y * Scale + translateY);
        }

        public SKPoint ScreenToWorld(SKPoint screenPoint) 
        {
            return new SKPoint((screenPoint.X - translateX) / _scale, (screenPoint.Y - translateY) / _scale);
        }

        public SKMatrix GetTransformMatrix()
        {
            return SKMatrix.CreateScale(Scale, Scale)
                .PostConcat(SKMatrix.CreateTranslation(translateX * DpiScale, translateY * DpiScale));
        }
    }
}
