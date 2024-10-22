<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//<canvas> 只支持两种基本形状：矩形和路径（由线连接的点列表）。
//所有其他形状必须通过组合一个或多个路径来创建。

var imageInfo = new SKImageInfo(600,600);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// 绘制一个矩形
var rect = new SKRect(100, 100, 300, 300);
using var paint = new SKPaint();
paint.Color = SKColors.Blue;
paint.Style = SKPaintStyle.Stroke;
canvas.DrawRect(rect, paint);

SKRect rect1 = new SKRect(0, 0, 100, 100);
SKRect rect2 = new SKRect(50, 50, 150, 150);

paint.Color = SKColors.Red;
canvas.DrawRect(rect1,paint);

// Inflate (膨胀)
rect1.Inflate(new SKSize(10, 10)); // 现在 rect1 变为 (-10, -10, 110, 110)  
canvas.DrawRect(rect1,paint);


// Intersect 创建相交矩形
rect1.Intersect(rect2);
paint.Color = SKColors.Green;
canvas.DrawRect(rect1,paint);

// IntersectsWith  
rect1.IntersectsWith(rect2).Dump("rect1 相交 rect2");

rect1 = new SKRect(0, 0, 100, 100);

// IntersectsWithInclusive  
rect1.IntersectsWith(rect).Dump("rect1 不相交 rect");
rect1.IntersectsWithInclusive(rect).Dump("rect1 带边框相交 rect");

var image = surfact.Snapshot();

image.Dump();



