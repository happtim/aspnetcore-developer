<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//可以使用系统默认的字体绘制。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);


using (var paint = new SKPaint())
{
	// 设置文本属性  
	paint.Color = SKColors.Black;
	paint.IsAntialias = true;
	paint.Style = SKPaintStyle.Fill;
	paint.TextAlign = SKTextAlign.Left;
	paint.TextSize = 32; // 设置字体大小  

	// 使用系统默认字体（通常支持中文）  
	paint.Typeface = SKTypeface.FromFamilyName("Microsoft YaHei", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

	// 如果你有特定的中文字体文件，可以这样加载：  
	// string fontPath = "path/to/your/chinese/font.ttf";  
	// paint.Typeface = SKTypeface.FromFile(fontPath);  

	// 绘制文本  
	canvas.DrawText("非零绕组规则", 10, 60, paint);

	// Draw the stroke text  
	canvas.DrawText("Hello world", 10, 100, paint);
}

var image = surfact.Snapshot();

image.Dump();

