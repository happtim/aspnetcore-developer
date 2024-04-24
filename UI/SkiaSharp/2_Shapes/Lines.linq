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

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// 创建一个 SKPaint 对象用于填充三角形
using (var fillPaint = new SKPaint())
{
	fillPaint.Style = SKPaintStyle.Fill; // 设置画笔为填充模式
	fillPaint.Color = SKColors.Black; // 设置画笔颜色为黑色

	// 创建一个路径对象
	using (var path = new SKPath())
	{
		path.MoveTo(25, 25); // 移动到起始点
		path.LineTo(105, 25); // 绘制第一条线
		path.LineTo(25, 105); // 绘制第二条线

		// 绘制填充三角形
		canvas.DrawPath(path, fillPaint);
	}
}

// 创建一个 SKPaint 对象用于描边三角形
using (var strokePaint = new SKPaint())
{
	strokePaint.Style = SKPaintStyle.Stroke; // 设置画笔为描边模式
	strokePaint.Color = SKColors.Black; // 设置画笔颜色为黑色
	strokePaint.StrokeWidth = 2; // 设置描边宽度

	// 创建另一个路径对象
	using (var path = new SKPath())
	{
		path.MoveTo(125, 125); // 移动到起始点
		path.LineTo(125, 45); // 绘制第一条线
		path.LineTo(45, 125); // 绘制第二条线
		path.Close(); // 关闭路径

		// 绘制描边三角形
		canvas.DrawPath(path, strokePaint);
	}
}

var image = surfact.Snapshot();

image.Dump();



