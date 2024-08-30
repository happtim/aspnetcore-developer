<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//创建一个线性渐变对象，其起始点为 (x1, y1)，终点为 (x2, y2)。
// SKColor[] colors:这是一个颜色数组，定义了渐变中使用的颜色。
//float[] colorPositions:这个数组定义了每种颜色在渐变中的位置。值的范围是 0.0 到 1.0，其中 0.0 表示起始点，1.0 表示结束点。这个数组的长度应该与 colors 数组的长度相同。

//SKShaderTileMode mode:这定义了渐变超出起始点和结束点后如何继续。
	//SKShaderTileMode.Clamp：在端点之外使用端点的颜色。
	//SKShaderTileMode.Repeat：重复渐变模式。
	//SKShaderTileMode.Mirror：镜像反射渐变模式。
var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// Create linear gradients  
using (SKPaint fillPaint = new SKPaint())
using (SKPaint strokePaint = new SKPaint())
{
	// Create first linear gradient  
	SKColor[] colors1 = new SKColor[]
	{ 
		SKColor.Parse("#00ABEB"),
		SKColors.White,
		SKColor.Parse("#26C000"),
		SKColors.White
	};
	float[] colorPositions1 = new float[] { 0f,0.5f , 0.5f, 1f};

	using (SKShader shader1 = SKShader.CreateLinearGradient(
		new SKPoint(0, 0),
		new SKPoint(0, 150),
		colors1,
		colorPositions1,
		SKShaderTileMode.Clamp))
	{
		fillPaint.Shader = shader1;

		// Create second linear gradient  
		SKColor[] colors2 = new SKColor[]
		{
			SKColors.Black,
			SKColors.Black.WithAlpha(0)
		};
		float[] colorPositions2 = new float[] { 0.5f, 1f };

		using (SKShader shader2 = SKShader.CreateLinearGradient(
			new SKPoint(0, 50),
			new SKPoint(0, 95),
			colors2,
			colorPositions2,
			SKShaderTileMode.Clamp))
		{
			strokePaint.Shader = shader2;
			strokePaint.Style = SKPaintStyle.Stroke;
			strokePaint.StrokeWidth = 2;  // Set a stroke width  

			// Draw shapes  
			canvas.DrawRect(10, 10, 130, 130, fillPaint);
			canvas.DrawRect(50, 50, 50, 50, strokePaint);
		}
	}
}

var image = surfact.Snapshot();

image.Dump();

