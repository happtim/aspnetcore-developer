<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"
//在裁剪区域内绘制几个不同的形状（红色矩形、绿色圆形和黄色矩形）。
//这些形状只在裁剪矩形内可见。
var imageInfo = new SKImageInfo(300,300);
using var surfact = SKSurface.Create(imageInfo);
var canvas = surfact.Canvas;

// 创建一个画笔  
using (var paint = new SKPaint
{
	Color = SKColors.Blue,
	Style = SKPaintStyle.Fill
})
{
	// 绘制一个背景矩形  
	canvas.DrawRect(0, 0, 300, 300, paint);

	// 定义一个裁剪矩形  
	SKRect clipRect = new SKRect(50, 50, 250, 250);

	// 保存当前的 canvas 状态  
	canvas.Save();

	// 应用裁剪矩形  
	canvas.ClipRect(clipRect);

	// 在裁剪区域内绘制  
	paint.Color = SKColors.Red;
	canvas.DrawRect(0, 0, 300, 300, paint);

	// 绘制一些其他形状，它们也会被裁剪  
	paint.Color = SKColors.Green;
	canvas.DrawCircle(150, 150, 100, paint);

	paint.Color = SKColors.Yellow;
	canvas.DrawRect(200, 200, 150, 150, paint);

	// 恢复 canvas 到之前的状态，移除裁剪  
	canvas.Restore();

	// 在裁剪区域外绘制，这部分不会被裁剪  
	paint.Color = SKColors.Purple;
	canvas.DrawRect(270, 270, 50, 50, paint);
}
var image = surfact.Snapshot();

image.Dump();



