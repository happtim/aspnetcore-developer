<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//除了在画布上绘制不透明的形状之外，我们还可以绘制半透明（或半透明）的形状。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

using (var paint = new SKPaint()) 
{
	 // Draw background
	paint.Color = new SKColor(255,221,0);
	canvas.DrawRect(0, 0, 150, 37.5f, paint);
	paint.Color = new SKColor(102, 204, 0);
	canvas.DrawRect(0, 37.5f, 150, 37.5f, paint);
	paint.Color = new SKColor(0, 153, 255);
	canvas.DrawRect(0, 75, 150, 37.5f, paint);
	paint.Color = new SKColor(255, 51, 0);
	canvas.DrawRect(0, 112.5f, 150, 37.5f, paint);

	// Draw semi transparent rectangles
	for (var i = 0; i < 10; i++)
	{
		paint.Color = new SKColor(255, 255, 255,(byte)((i + 1) / 10f * 250));
		for (var j = 0; j < 4; j++)
		{
			canvas.DrawRect(5 + i * 14, 5 + j * 37.5f, 14, 27.5f, paint);
		}
	}
}


var image = surfact.Snapshot();

image.Dump();

