<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//正如您在前面的示例中所看到的，当使用斜接选项连接两条线时，这两条线的外边缘会延伸到它们相遇的点。对于彼此夹角较大的线条，这一点离内连接点并不远。
//然而，随着每条线之间的夹角减小，这些点之间的距离（斜接长度）呈指数增长。
//miterLimit属性确定外连接点可以距离内连接点的距离。如果两条线超过此值，将绘制斜角连接。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// Draw guides  
using (SKPaint guidePaint = new SKPaint
{
	Color = new SKColor(0, 153, 255),
	StrokeWidth = 2,
	Style = SKPaintStyle.Stroke
})
{
	canvas.DrawRect(-5, 50, 160, 50, guidePaint);
}

// Set line styles  
using (SKPaint linePaint = new SKPaint
{
	Color = SKColors.Black,
	StrokeWidth = 10,
	Style = SKPaintStyle.Stroke,
	StrokeJoin = SKStrokeJoin.Miter,
	StrokeMiter = 9,
})
{
	// Draw lines  
	using (SKPath path = new SKPath())
	{
		path.MoveTo(0, 100);
		for (int i = 0; i < 24; i++)
		{
			float dy = i % 2 == 0 ? 25 : -25;
			path.LineTo((float)Math.Pow(i, 1.5) * 2, 75 + dy);
		}
		canvas.DrawPath(path, linePaint);
	}
}

var image = surfact.Snapshot();

image.Dump();

