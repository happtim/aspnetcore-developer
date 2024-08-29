<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//我们就可以使用drawImage()方法将其渲染到画布上。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

string imagePath = Path.GetDirectoryName( Util.CurrentQueryPath) + "\\backdrop.png";

using var stream = new FileStream(imagePath, FileMode.Open);

var bitmap =  SKBitmap.Decode(stream);

// 绘制图片  
canvas.DrawBitmap(bitmap, 0, 0);

// 创建路径  
using (var path = new SKPath())
{
	path.MoveTo(30, 96);
	path.LineTo(70, 66);
	path.LineTo(103, 76);
	path.LineTo(170, 15);

	// 创建画笔  
	using (var paint = new SKPaint())
	{
		paint.Style = SKPaintStyle.Stroke;
		paint.Color = SKColors.Red;
		paint.StrokeWidth = 2;
		paint.IsAntialias= true;

		// 绘制路径  
		canvas.DrawPath(path, paint);
	}
}

var image = surfact.Snapshot();

image.Dump();

