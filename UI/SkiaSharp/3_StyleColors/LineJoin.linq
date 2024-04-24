<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//lineJoin 属性确定了在形状中连接两个具有非零长度的线段、弧线或曲线时它们如何连接在一起

//miter：连接的线段通过延伸它们的外边缘以连接到一个单点，从而填充了一个额外的菱形区域。
//round：线条的末端是圆形的。
//bevel：填充连接线段的公共端点与每个线段的独立外部矩形角之间的额外三角区域。

//StrokeMiter:miter选项两条连接线的外边缘会延伸到它们相遇的点。随着每条线之间的角度减小，这些点之间的距离（斜接长度）呈指数增加。
//需要有参数限制该较的距离。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

using (var paint = new SKPaint()) 
{
	paint.StrokeWidth = 10;
	paint.Style = SKPaintStyle.Stroke;
	for(int i = 0; i< 3;i++)
	{	
		using (var path = new SKPath()) 
		{
			paint.StrokeJoin = (SKStrokeJoin)i;
			path.MoveTo(new SKPoint(-5, 5 + i * 40));
			path.LineTo(new SKPoint(35, 45 + i * 40));
			path.LineTo(new SKPoint(75, 5+ i * 40));
			path.LineTo(new SKPoint(115,45 + i * 40));
			path.LineTo(new SKPoint(155,5 + i * 40));
			
			canvas.DrawPath(path,paint);
		}
	}
	
}


var image = surfact.Snapshot();

image.Dump();

