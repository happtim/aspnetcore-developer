<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//在这个例子中，我们将定义两种不同的锥形渐变。锥形渐变与径向渐变不同，它不是创建圆圈，而是围绕一个点旋转。

//参数:
//SKPoint center:这是渐变的中心点。
//SKColor[] colors:这是一个颜色数组，定义了渐变中使用的颜色。
//float[] colorPositions:这个数组定义了每种颜色在渐变中的位置。值的范围是 0.0 到 1.0，表示渐变的完整旋转。



var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// Create gradients  
using SKShader conicGrad1 = SKShader.CreateSweepGradient(
	new SKPoint(62, 75),
	new SKColor[] { SKColor.Parse("#A7D30C"), SKColors.White},
	new float[] { 0, 1 }
);

using SKShader conicGrad2 = SKShader.CreateSweepGradient(
	new SKPoint(187, 75),
	new SKColor[] { SKColors.Black, SKColors.Black, SKColors.White, SKColors.White,
					SKColors.Black, SKColors.Black, SKColors.White, SKColors.White },
	new float[] { 0, 0.25f, 0.25f, 0.5f, 0.5f, 0.75f, 0.75f, 1 }
);

// Draw shapes  
using (SKPaint paint = new SKPaint())
{
	paint.IsAntialias = true;
	// First gradient  
	SKMatrix matrix = SKMatrix.CreateRotation(2,62,75);
	
	paint.Shader = conicGrad1.WithLocalMatrix(matrix);
	canvas.DrawRect(12, 25, 100, 100, paint);
	
	// Second gradient  
	paint.Shader = conicGrad2;
	canvas.DrawRect(137, 25, 100, 100, paint);
}


var image = surfact.Snapshot();

image.Dump();

