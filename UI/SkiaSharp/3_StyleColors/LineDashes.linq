<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//PathEffect：属性指定了线条的虚线模式。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

using (var paint = new SKPaint()) 
{
	//第一参数 表示虚线的线段和间隙的长度。第二个参数 0 表示起始偏移量。
	paint.PathEffect = SKPathEffect.CreateDash(new float[] {4,2},0);

	// 在画布上绘制一条带有虚线效果的直线
	SKPoint startPoint = new SKPoint(100, 100);
	SKPoint endPoint = new SKPoint(300, 100);
	
	canvas.DrawLine(startPoint, endPoint, paint);
}

var image = surfact.Snapshot();

image.Dump();

