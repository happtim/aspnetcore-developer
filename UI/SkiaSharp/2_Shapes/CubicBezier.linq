<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//CubicTo 绘制三次贝塞尔曲线
//它需要三个点：前两个是控制点，第三个是终点。起始点是当前路径中的最后一个点，

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

using (var paint = new SKPaint()) 
{
	var start = new SKPoint { X = 50, Y = 20 };
	var cp1 = new SKPoint { X = 230, Y = 30 };
	var cp2 = new SKPoint { X = 150, Y = 80 };
	var end = new SKPoint { X = 250, Y = 100};
	
	using (var path = new SKPath()) 
	{

		// Cubic Bézier curve
		path.MoveTo(start);
		path.CubicTo(cp1,cp2,end);
		paint.Style = SKPaintStyle.Stroke;
		canvas.DrawPath(path,paint);
		
		// Start and end points
		paint.Color = SKColors.Blue;
		paint.Style = SKPaintStyle.Fill;
		canvas.DrawCircle(start,5,paint);
		canvas.DrawCircle(end,5,paint);
		
		// Control points
		paint.Color = SKColors.Red;
		canvas.DrawCircle(cp1,5,paint); // Control point one
		canvas.DrawCircle(cp2,5,paint); // Control point two
	}
}


var image = surfact.Snapshot();

image.Dump();

