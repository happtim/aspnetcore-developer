<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//实际上并不绘制任何东西，但它成为了上述路径列表的一部分，这个功能是 moveTo() 。
//你可以把它想象成将笔或铅笔从纸上的一个位置抬起，然后放在下一个位置上。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// 绘制笑脸
using (var paint = new SKPaint() { IsAntialias = true})
{
	// 绘制外圆
	paint.Color = SKColors.Black;
	paint.Style = SKPaintStyle.Stroke;
	
	canvas.DrawCircle(100, 100, 80, paint);

	// 绘制眼睛
	canvas.DrawCircle(70, 80, 20, paint);
	canvas.DrawCircle(130, 80, 20, paint);

	// 绘制嘴巴
	using (var path = new SKPath())
	{
		// Start point for the mouth
		path.MoveTo(110, 75);

		// The rectangle that bounds the arc
		var rect = new SKRect(50, 70, 150, 160);

		// Adding the arc to the path
		// Note: SkiaSharp's ArcTo does not directly support the startAngle, endAngle, and direction parameters like HTML Canvas
		// We're using an oval and specifying the start and sweep angles to simulate the mouth
		path.ArcTo(rect, 0, 180, true);
		
		path.d

		// Draw the path
		canvas.DrawPath(path, paint);
	}
}

var image = surfact.Snapshot();

image.Dump();



