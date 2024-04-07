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

SKRect oval = new SKRect(4,4,60,60);

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

var paint = new SKPaint();

foreach (var userCenter in new bool[] { true, false})
{
	foreach (var style in new SKPaintStyle[] { SKPaintStyle.Stroke,SKPaintStyle.Fill})
	{
		paint.Style = style;
		foreach (var degree in new int[] {45,90,180,360})
		{
			canvas.DrawArc(oval,0,degree,userCenter,paint);
			canvas.Translate(64,0);
		}
		canvas.Translate(-256,64);
	}
}

var image = surfact.Snapshot();

image.Dump();



