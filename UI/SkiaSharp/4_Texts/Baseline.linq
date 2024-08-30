<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//在 SkiaSharp 中，没有直接对应 HTML5 Canvas 的 textBaseline 属性的设置。但是，我们可以通过计算字体度量（font metrics）来模拟不同的 baseline 行为。
//我们可以使用 SKPaint.FontMetrics 来获取字体的各种度量信息，然后根据需要调整文本的绘制位置。

//基线对齐设置。可能的值：top，hanging，middle，alphabetic，ideographic，bottom。默认值为alphabetic。

var imageInfo = new SKImageInfo(600,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);


DrawTextWithBaseline(canvas, "Top", 10, 100, TextBaseline.Top);
DrawTextWithBaseline(canvas, "Hanging", 10, 200, TextBaseline.Hanging);
DrawTextWithBaseline(canvas, "Middle", 10, 300, TextBaseline.Middle);
DrawTextWithBaseline(canvas, "Alphabetic", 300, 100, TextBaseline.Alphabetic);
DrawTextWithBaseline(canvas, "Ideographic", 300, 200, TextBaseline.Ideographic);
DrawTextWithBaseline(canvas, "Bottom", 300, 300, TextBaseline.Bottom);

var image = surfact.Snapshot();

image.Dump();

void DrawTextWithBaseline(SKCanvas canvas, string text, float x, float y, TextBaseline baseline)
{
	using (SKPaint paint = new SKPaint())
	{
		paint.Color = SKColors.Black;
		paint.IsAntialias = true;
		paint.TextSize = 48;
		paint.Typeface = SKTypeface.FromFamilyName("serif");
		paint.Style = SKPaintStyle.Stroke;
		paint.StrokeWidth = 1;

		SKFontMetrics metrics = paint.FontMetrics;
		float baselineOffset = 0;

		switch (baseline)
		{
			case TextBaseline.Top:
			baselineOffset = -metrics.Ascent;
			break;
			case TextBaseline.Hanging:
			baselineOffset = -metrics.Ascent * 0.8f; // Approximation  
			break;
			case TextBaseline.Middle:
			baselineOffset = (-metrics.Ascent - metrics.Descent) / 2;
			break;
			case TextBaseline.Alphabetic:
			baselineOffset = 0; // Default  
			break;
			case TextBaseline.Ideographic:
			baselineOffset = -metrics.Descent;
			break;
			case TextBaseline.Bottom:
			baselineOffset = -metrics.Descent;
			break;
		}

		// Draw baseline  
		canvas.DrawLine(x, y, x + 200, y, paint);

		// Draw text  
		canvas.DrawText(text, x, y - baselineOffset, paint);
	}
}

enum TextBaseline
{
	Top,
	Hanging,
	Middle,
	Alphabetic,
	Ideographic,
	Bottom
}