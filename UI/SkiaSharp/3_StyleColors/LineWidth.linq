<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//绘制了10条直线，线宽逐渐增加。
//从最左边和所有其他奇数宽度的线条由于路径的定位而显得不清晰。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

using (var paint = new SKPaint()) 
{
	for (int i = 0; i < 10; i++)
	{
		paint.StrokeWidth = 1 + i;
		canvas.DrawLine(5 + i*14 , 5, 5+i*14, 140, paint);
	}
}


var image = surfact.Snapshot();

image.Dump();

