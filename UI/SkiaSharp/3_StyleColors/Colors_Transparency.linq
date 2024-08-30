<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//在这个例子中，我们将绘制四个不同颜色的正方形背景。在这些正方形上面，我们将绘制一组半透明的圆。
//在for循环中的每一步都会绘制一组半径逐渐增大的圆。最终结果是一个径向渐变。
//通过将越来越多的圆叠加在一起，我们有效地降低了已经绘制的圆的透明度。通过增加步数并实际绘制更多的圆，背景将完全从图像中心消失。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

// Draw background  
using (SKPaint paint = new SKPaint())
{
	paint.Color = SKColor.Parse("#FFDD00"); // #FD0  
	canvas.DrawRect(0, 0, 75, 75, paint);

	paint.Color = SKColor.Parse("#66CC00"); // #6C0  
	canvas.DrawRect(75, 0, 75, 75, paint);

	paint.Color = SKColor.Parse("#0099FF"); // #09F  
	canvas.DrawRect(0, 75, 75, 75, paint);

	paint.Color = SKColor.Parse("#FF3300"); // #F30  
	canvas.DrawRect(75, 75, 75, 75, paint);

	// Set color and transparency for circles  
	paint.Color = SKColors.White;
	paint.Style = SKPaintStyle.Fill;

	// Draw semi transparent circles  
	for (int i = 0; i < 7; i++)
	{
		paint.Color = paint.Color.WithAlpha(51); // 0.2 * 255 ≈ 51  
		canvas.DrawCircle(75, 75, 10 + 10 * i, paint);
	}
}

var image = surfact.Snapshot();

image.Dump();

