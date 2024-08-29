<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//下一个转换方法是缩放。我们使用它来增加或减少画布网格中的单位。这可以用来绘制缩小或放大的形状和位图。
//scale(x, y) 按水平方向和垂直方向分别按 x 和 y 缩放画布单位。两个参数都是实数。小于 1.0 的值会减小单位大小，大于 1.0 的值会增加单位大小。值为 1.0 时单位大小保持不变。

//使用负数，您可以进行轴镜像 (for example using translate(0,canvas.height); scale(1,-1);)

//默认情况下，画布上的一个单位正好是一个像素。例如，如果我们应用缩放因子 0.5，那么结果单位将变为 0.5 像素，因此形状将以一半的大小绘制。
//类似地，将缩放因子设置为 2.0 会增加单位大小，一个单位现在变为两个像素。这导致形状被绘制为两倍大小。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// draw a simple rectangle, but scale it.  
canvas.Save();
canvas.Scale(10, 3);
using (var paint = new SKPaint())
{
	canvas.DrawRect(1, 10, 10, 10, paint);
}
canvas.Restore();

// mirror horizontally  
canvas.Scale(-1, 1);
using (var paint = new SKPaint
{
	TextSize = 48,
	Typeface = SKTypeface.FromFamilyName("serif"),
	IsAntialias = true
})
{
	canvas.DrawText("MDN", -135, 120, paint);
}

var image = surfact.Snapshot();

image.Dump();

