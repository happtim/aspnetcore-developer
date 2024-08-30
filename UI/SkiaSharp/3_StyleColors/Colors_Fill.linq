<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//可以使用Paint中Color设置画笔的颜色

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

using (var paint = new SKPaint()) 
{
	for (int i = 0; i < 6; i++)
	{
		for(int j = 0; j < 6; j++)
		{
			paint.Color = new SKColor((byte)( 255-42.5*i),(byte)(255-42.5*j),0);
			canvas.DrawRect(j*25,i*25,25,25,paint);
		}
	}
}


var image = surfact.Snapshot();

image.Dump();

