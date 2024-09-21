<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//我们可以使用CreateBitmap来循环创建图像

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 加载图片  
string imagePath = Path.GetDirectoryName(Util.CurrentQueryPath) + "\\canvas_createpattern.png";

using var stream = new FileStream(imagePath, FileMode.Open);

using var bitmap = SKBitmap.Decode(stream);

// 创建图案  
using (SKShader shader = SKShader.CreateBitmap(bitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat))
{
	// 创建画笔并设置图案  
	using (SKPaint paint = new SKPaint())
	{
		paint.Shader = shader;

		// 绘制矩形  
		canvas.DrawRect(0, 0, 150, 150, paint);
	}
}


var image = surfact.Snapshot();

image.Dump();

