<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

 #load "..\Dump"
 
 // 设置图像的宽度和高度
int width = 300;
int height = 300;

    // 创建一个 SKBitmap 对象
using var bitmap = new SkiaSharp.SKBitmap(width, height);

// 创建一个 SKCanvas 对象
using (var canvas = new SkiaSharp.SKCanvas(bitmap))
{
    // 设置背景为白色
    canvas.Clear(SkiaSharp.SKColors.White);

    // 创建一个用于绘制的 SKPaint 对象
    var paint = new SkiaSharp.SKPaint
    {
        Color = SkiaSharp.SKColors.Blue, // 设置画笔颜色
        IsAntialias = true, // 设置抗锯齿
    };

    // 在画布上绘制一个圆
    canvas.DrawCircle(width / 2, height / 2, 100, paint);

    // 修改画笔颜色和属性，用于绘制文本
    paint.Color = SkiaSharp.SKColors.Black;
    paint.TextSize = 32;

    // 在画布上绘制文本
    canvas.DrawText("Hello, SkiaSharp!", 25, height / 2 + 100, paint);
}

// 将 SKBitmap 对象保存为图片文件
using var image = SkiaSharp.SKImage.FromBitmap(bitmap);
image.Dump();

