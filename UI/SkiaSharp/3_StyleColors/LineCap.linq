<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//我们将绘制三条线，每条线的属性值都不同。
//lineCap 属性确定了每条线的端点如何绘制。
//butt：线的末端在端点处是方形的。
//round：线条的末端是圆形的。
//square：线条的末端通过添加一个宽度相等且高度为线条厚度一半的方框来方正。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

using (var paint = new SKPaint()) 
{
	// Draw guides
	paint.Color = SKColor.Parse("#09f");
	canvas.DrawLine(new SKPoint(10,10),new SKPoint(140,10),paint);
	canvas.DrawLine(new SKPoint(10,140),new SKPoint(140,140),paint);
	
	// Draw lines
	for (int i = 0; i < 3 ; i++)
	{
		paint.StrokeWidth = 15;
		paint.Color = SKColors.Black;
		paint.StrokeCap = (SKStrokeCap) i;
		canvas.DrawLine(new SKPoint(25 + i * 50, 10 ),new SKPoint(25 + i * 50 ,140),paint);
	}
	
}


var image = surfact.Snapshot();

image.Dump();

