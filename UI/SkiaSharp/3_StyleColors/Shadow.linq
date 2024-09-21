<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//使用阴影只涉及四个属性：

//dx(float)：
//	阴影在 X 轴方向的偏移量。
//	正值使阴影向右移动，负值使阴影向左移动。

//dy(float)：
//	阴影在 Y 轴方向的偏移量。
//	正值使阴影向下移动，负值使阴影向上移动。

//sigmaX(float)：
//	阴影在 X 轴方向的模糊程度。
//	值越大，阴影在水平方向越模糊。

//sigmaY(float)：
//	阴影在 Y 轴方向的模糊程度。
//	值越大，阴影在垂直方向越模糊。

//color(SKColor)：
//	阴影的颜色。
//	使用 SKColor 对象来指定，包括 RGB 值和 Alpha 通道（透明度）。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// 创建画笔  
using (SKPaint paint = new SKPaint())
{
	// 设置字体  
	paint.Typeface = SKTypeface.FromFamilyName("Times New Roman");
	paint.TextSize = 20;
	paint.Color = SKColors.Black;

	// 设置阴影  
	paint.ImageFilter = SKImageFilter.CreateDropShadow(
		dx: 2,
		dy: 2,
		sigmaX: 2,
		sigmaY: 2,
		color: new SKColor(0, 0, 0, 128)
	);

	// 绘制文本  
	canvas.DrawText("Sample String", 5, 30, paint);
}

var image = surfact.Snapshot();

image.Dump();

