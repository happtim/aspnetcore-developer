<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//在这个例子中，我们将定义四种不同的径向渐变。
//由于我们可以控制渐变的起始点和结束点，所以我们可以实现比我们通常在“经典”径向渐变中看到的更复杂的效果

//参数：
//SKPoint center:这是渐变的中心点。
//float radius:这是渐变的半径。
//SKColor[] colors:这是一个颜色数组，定义了渐变中使用的颜色。
//float[] colorPositions:这个数组定义了每种颜色在渐变中的位置。值的范围是 0.0 到 1.0，其中 0.0 表示中心点，1.0 表示半径边缘。
//SKShaderTileMode mode:
	//SKShaderTileMode.Clamp：在半径之外使用边缘的颜色。
	//SKShaderTileMode.Repeat：重复渐变模式。
	//SKShaderTileMode.Mirror：镜像反射渐变模式。


var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// Create radial gradients  
using SKShader radgrad = SKShader.CreateRadialGradient(
	new SKPoint(45, 45),
	30,
	new SKColor[] { SKColor.Parse("#A7D30C"), SKColor.Parse("#019F62"), SKColor.Parse("#019F62").WithAlpha(0) },
	new float[] { 0, 0.9f, 1 },
	SKShaderTileMode.Clamp);

using SKShader radgrad2 = SKShader.CreateRadialGradient(
	new SKPoint(105, 105),
	50,
	new SKColor[] { SKColor.Parse("#FF5F98"), SKColor.Parse("#FF0188"), SKColor.Parse("#FF0188").WithAlpha(0) },
	new float[] { 0, 0.75f, 1 },
	SKShaderTileMode.Clamp);

using SKShader radgrad3 = SKShader.CreateRadialGradient(
	new SKPoint(95, 15),
	40,
	new SKColor[] { SKColor.Parse("#00C9FF"), SKColor.Parse("#00B5E2"), SKColor.Parse("#00C9FF").WithAlpha(0) },
	new float[] { 0, 0.8f, 1 },
	SKShaderTileMode.Clamp);

using SKShader radgrad4 = SKShader.CreateRadialGradient(
	new SKPoint(0, 150),
	90,
	new SKColor[] { SKColor.Parse("#F4F201"), SKColor.Parse("#E4C700"), SKColor.Parse("#E4C700").WithAlpha(0) },
	new float[] { 0, 0.8f, 1 },
	SKShaderTileMode.Clamp);

// Draw shapes  
using (SKPaint paint = new SKPaint())
{
	paint.Shader = radgrad4;
	canvas.DrawRect(0, 0, 150, 150, paint);

	paint.Shader = radgrad3;
	canvas.DrawRect(0, 0, 150, 150, paint);

	paint.Shader = radgrad2;
	canvas.DrawRect(0, 0, 150, 150, paint);

	paint.Shader = radgrad;
	canvas.DrawRect(0, 0, 150, 150, paint);
}

var image = surfact.Snapshot();

image.Dump();

