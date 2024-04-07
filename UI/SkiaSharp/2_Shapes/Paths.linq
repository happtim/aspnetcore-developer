<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//路径是一系列点的列表，通过线段连接，线段可以是不同形状的，可以是曲线或直线，
//可以是不同宽度和不同颜色的。
//可以是子路径，
//可以是闭合的。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// 绘制一个三角形
using (var paint = new SKPaint())
{
	paint.Color = SKColors.Blue;
	paint.Style = SKPaintStyle.Fill;

	var path = new SKPath();
	path.MoveTo(200, 100);
	path.LineTo(300, 300);
	path.LineTo(100, 300);
	path.Close();

	canvas.DrawPath(path, paint);
}


var image = surfact.Snapshot();

image.Dump();



