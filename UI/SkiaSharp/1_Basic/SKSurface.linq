<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

var imageInfo = new SKImageInfo(300,300);
using var surfact = SKSurface.Create(imageInfo);
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// 创建一个用于绘制的 SKPaint 对象
var paint = new SkiaSharp.SKPaint
{
	Color = SkiaSharp.SKColors.Blue, // 设置画笔颜色
	IsAntialias = true, // 设置抗锯齿
};

// 在画布上绘制一个圆
canvas.DrawCircle(300 / 2, 300 / 2, 100, paint);

// 修改画笔颜色和属性，用于绘制文本
paint.Color = SkiaSharp.SKColors.Black;
paint.TextSize = 32;

// 在画布上绘制文本
canvas.DrawText("Hello, SkiaSharp!", 25, 300 / 2 + 100, paint);

var image = surfact.Snapshot();

image.Dump();



