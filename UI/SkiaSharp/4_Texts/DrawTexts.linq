<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//可以绘制文字

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);


// Create a paint object for the text  
using (SKPaint paint = new SKPaint())
{
	paint.Color = SKColors.Black;
	paint.IsAntialias = true;
	paint.TextSize = 48;
	paint.Typeface = SKTypeface.FromFamilyName("serif");

	// Draw the text  
	canvas.DrawText("Hello world", 10, 50, paint);
}

// Create a paint object for the text  
using (SKPaint paint = new SKPaint())
{
	paint.Color = SKColors.Black;
	paint.IsAntialias = true;
	paint.TextSize = 48;
	paint.Typeface = SKTypeface.FromFamilyName("serif");
	paint.Style = SKPaintStyle.Stroke;  // Set to stroke style  
	paint.StrokeWidth = 1;  // Set the stroke width (adjust as needed)  

	// Draw the stroke text  
	canvas.DrawText("Hello world", 10, 100, paint);
}

var image = surfact.Snapshot();

image.Dump();

