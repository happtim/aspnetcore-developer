<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//用 ArcTo() 方法绘制弧线。这种方法通常用于制作圆角。
//起始点和控制点在一条直线上，圆弧会自动与路径的最新点连接成一条直线。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

using (var paint = new SKPaint()) 
{
	paint.IsAntialias = true;
	paint.IsStroke = true;
	
	// Tangential lines
	using (var path = new SKPath())
	{
		
		path.MoveTo(200,20);
		path.LineTo(200,130);
		path.LineTo(50,20);
		canvas.DrawPath(path,paint);
	}

	// Arc
	using (var path = new SKPath()) 
	{
		paint.StrokeWidth = 5;
		//如果最后一个路径点不是以弧线开始，arcTo会将连接线附加到路径上。
		path.MoveTo(200,20);
		
		path.ArcTo(200,130,50,20,40);
		
		//弧度扫描始终小于180度。arcTo将从最后一个路径点添加直线到（x1，y1）。
		//path.ArcTo(200,130,220,220,40);
		
		//半径为零，arcTo将从最后一个路径点添加直线到（x1，y1）。
		//path.ArcTo(200,130,50,20,0);
		
		//切线平行，arcTo将从最后一个路径点添加直线到（x1，y1）。
		//path.ArcTo(200,130,200,0,40);
		
		canvas.DrawPath(path,paint);
	}
	

	// Start point
	using (var path = new SKPath()) 
	{
		paint.Style = SKPaintStyle.Fill;
		paint.Color = SKColors.Blue;
		path.AddCircle(200,20,5);
		canvas.DrawPath(path,paint);
	}

	// Control points
	using (var path = new SKPath())
	{
		paint.Color = SKColors.Red;
		path.AddCircle(200,130,5);// Control point one
		path.AddCircle(50,20,5);// Control point two
		canvas.DrawPath(path, paint);
	}
}


var image = surfact.Snapshot();

image.Dump();

