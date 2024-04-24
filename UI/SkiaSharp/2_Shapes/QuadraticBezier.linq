<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//QuadTo 绘制二次贝塞尔曲线
//它需要两个点：第一个是控制点，第二个是结束点。起始点是当前路径中的最新点

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

using (var paint = new SKPaint()) 
{
	using (var path = new SKPath()) 
	{

		// Quadratic Bézier curve
		path.MoveTo(50,20);
		path.QuadTo(230,30,50,100);
		paint.Style = SKPaintStyle.Stroke;
		canvas.DrawPath(path,paint);
		
		paint.Color = SKColors.Blue;
		paint.Style = SKPaintStyle.Fill;
		canvas.DrawCircle(50,20,5,paint);
		canvas.DrawCircle(50,100,5,paint);
		
		paint.Color = SKColors.Red;
		canvas.DrawCircle(230,30,5,paint);
	}
}


var image = surfact.Snapshot();

image.Dump();

