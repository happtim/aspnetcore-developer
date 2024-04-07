<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//<canvas> 只支持两种基本形状：矩形和路径（由线连接的点列表）。
//所有其他形状必须通过组合一个或多个路径来创建。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// 绘制一个矩形
var rect = new SKRect(100, 100, 300, 300);
using (var paint = new SKPaint())
{
	paint.Color = SKColors.Blue;
	canvas.DrawRect(rect, paint);
}

var image = surfact.Snapshot();

image.Dump();



