<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//第二个转换方法是rotate()。我们使用它来围绕当前原点旋转画布。
//旋转画布，使其以当前原点为中心顺时针旋转角度（弧度）。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// left rectangles, rotate from canvas origin  
canvas.Save();
// blue rect  
using (var paint = new SKPaint { Color = SKColor.Parse("#0095DD") })
{
	canvas.DrawRect(30, 30, 100, 100, paint);
}
canvas.RotateDegrees(25);
// grey rect  
using (var paint = new SKPaint { Color = SKColor.Parse("#4D4E53") })
{
	canvas.DrawRect(30, 30, 100, 100, paint);
}
canvas.Restore();

// right rectangles, rotate from rectangle center  
// draw blue rect  
using (var paint = new SKPaint { Color = SKColor.Parse("#0095DD") })
{
	canvas.DrawRect(150, 30, 100, 100, paint);
}

canvas.Translate(200, 80); // translate to rectangle center  
canvas.RotateDegrees(25); // rotate  
canvas.Translate(-200, -80); // translate back  

// draw grey rect  
using (var paint = new SKPaint { Color = SKColor.Parse("#4D4E53") })
{
	canvas.DrawRect(150, 30, 100, 100, paint);
}

var image = surfact.Snapshot();

image.Dump();

