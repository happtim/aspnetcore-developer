<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//为了在形状中画一个孔，我们需要在画外形时以不同的时钟方向画孔。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

using (var paint = new SKPaint()) 
{
	
	using (var path = new SKPath()) 
	{
		// Outter shape clockwise ⟳
		path.MoveTo(0,0);
		path.LineTo(150,0);
		path.LineTo(75,129.9f);
		
		 // Inner shape anticlockwise ↺
		 path.MoveTo(75,20);
		 path.LineTo(50,60);
		 path.LineTo(100,60);
		 
		 canvas.DrawPath(path,paint);
		
	}
}


var image = surfact.Snapshot();

image.Dump();

