<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//当使用 fill（或 clip 和 isPointInPath）时，您可以选择性地提供一个填充规则算法
//，用于确定点是否在路径内部或外部，从而确定是否填充。

//默认使用 非零绕组规则

//在二维计算机图形中，非零环绕规则是确定给定点是否落在封闭曲线内的一种方法。
//与类似的奇偶规则不同，它依赖于知道曲线每个部分的描边方向。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

using (SKPath path = new SKPath())
{
	// 绘制外圆  
	path.AddCircle(50, 50, 30);

	// 绘制内圆  
	path.AddCircle(50, 50, 15);

	// 创建画笔  
	using (SKPaint paint = new SKPaint())
	{
		// SKPathFillType.EvenOdd 确定了填充规则为奇偶规则，这样内圆将被从填充区域中“挖出”，形成一个环形。
		path.FillType = SKPathFillType.EvenOdd;

		// 填充路径  
		canvas.DrawPath(path, paint);
	}
}


var image = surfact.Snapshot();

image.Dump();

