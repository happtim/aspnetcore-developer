<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//使用非系统默认的字体绘制。

//如使用google开源的中文字体。
//访问 https://fonts.google.com/noto/specimen/Noto+Sans+SC
//点击 "Download family"
//解压下载的文件，找到 "NotoSansSC-Regular.ttf" 文件


var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);


string fontPath = Path.GetDirectoryName( Util.CurrentQueryPath) + "\\NotoSansSC-Regular.ttf";

// 确保字体文件存在  
if (!File.Exists(fontPath))
{
	Console.WriteLine("字体文件不存在：" + fontPath);
	return;
}

// 创建画笔  
using (var paint = new SKPaint())
{
	// 加载 Google Noto Sans SC 字体  
	using (var typeface = SKTypeface.FromFile(fontPath))
	{
		// 设置文本属性  
		paint.Typeface = typeface;
		paint.Color = SKColors.Black;
		paint.IsAntialias = true;
		paint.Style = SKPaintStyle.Fill;
		paint.TextAlign = SKTextAlign.Left;
		paint.TextSize = 32; // 设置字体大小  

		// 绘制文本  
		canvas.DrawText("非零绕组规则", 10, 60, paint);

		// Draw the stroke text  
		canvas.DrawText("Hello world", 10, 100, paint);
	}
}

var image = surfact.Snapshot();

image.Dump();

