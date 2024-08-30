<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//但使用strokeStyle属性来改变形状轮廓的颜色。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

using (SKPaint paint = new SKPaint())
{
	paint.Style = SKPaintStyle.Stroke;
	paint.IsAntialias = true;
	paint.StrokeWidth = 1;

	for (int i = 0; i < 6; i++)
	{
		for (int j = 0; j < 6; j++)
		{
			byte green = (byte)(255 - 42.5 * i);
			byte blue = (byte)(255 - 42.5 * j);
			paint.Color = new SKColor(0, green, blue);

			float x = 12.5f + j * 25;
			float y = 12.5f + i * 25;

			canvas.DrawCircle(x, y, 10, paint);
		}
	}
}


var image = surfact.Snapshot();

image.Dump();

