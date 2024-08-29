<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//第三个也是最后一个drawImage()方法的变体除了图像源之外还有八个参数。它允许我们剪切源图像的一部分，然后在画布上缩放并绘制它。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

string rhinoPath = Path.GetDirectoryName( Util.CurrentQueryPath) + "\\rhino.jpg";

using var stream = new FileStream(rhinoPath, FileMode.Open);

var rhino =  SKBitmap.Decode(stream);

// 定义要裁剪的区域（源矩形）  
SKRect sourceRect = new SKRect(50, 70, 140, 190);  // 从原图的(50,70)开始，裁剪90x120的区域  

// 定义绘制区域（目标矩形）  
SKRect destRect = new SKRect(20,20, 110, 140);  // 在画布上(20,20)位置绘制，大小为90x120

// 绘制裁剪后的图片  
canvas.DrawBitmap(rhino, sourceRect, destRect);


string framePath = Path.GetDirectoryName(Util.CurrentQueryPath) + "\\canvas_picture_frame.png";

using var stream2 = new FileStream(framePath, FileMode.Open);

var frame = SKBitmap.Decode(stream2);

canvas.DrawBitmap(frame,0,0);

var image = surfact.Snapshot();

image.Dump();

