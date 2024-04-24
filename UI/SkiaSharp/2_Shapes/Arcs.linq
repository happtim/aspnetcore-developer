<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//用 lineTo() 方法画直线。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

SKRect oval = new SKRect(4,4,60,60);

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

for (int i = 0; i < 4; i++)
{
	for (int j = 0; j < 3; j++)
	{
		var x = 25 + j * 50; // x coordinate
		var y = 25 + i * 50; // y coordinate
		var radius = 20; // Arc radius
		var endAngle = Math.PI + (Math.PI * j) / 2; // End point on circle
		var counterclockwise = i % 2 != 0; // clockwise or counterclockwise
		
		using (var paint = new SKPaint())
		{
			paint.Style = i > 1 ? SKPaintStyle.Fill : SKPaintStyle.Stroke;
			paint.Color = SKColors.Black;
			paint.StrokeWidth = 1;
			paint.IsAntialias = true;

			using (var path = new SKPath())
			{
				path.Arc(x, y, radius, 0, (float)(endAngle), counterclockwise);
				canvas.DrawPath(path, paint);
			}
		}
	}
}

var image = surfact.Snapshot();

image.Dump();

