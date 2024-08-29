<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//这种方法用于将画布及其原点移动到网格中的不同点。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

for (int i = 0; i < 3; i++)
{
	for (int j = 0; j < 3; j++)
	{
		using (var paint = new SKPaint())
		{
			paint.Color = new SKColor((byte)(51 * i), (byte)(255 - 51 * i), 255);

			canvas.Save();
			canvas.Translate(10 + j * 50, 10 + i * 50);
			canvas.DrawRect(new SKRect(0, 0, 25, 25), paint);
			canvas.Restore();
		}
	}
}

var image = surfact.Snapshot();

image.Dump();

