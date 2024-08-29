<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//第二个drawImage()方法的变体添加了两个新参数，并允许我们在画布上放置缩放后的图像。

//

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

string imagePath = Path.GetDirectoryName( Util.CurrentQueryPath) + "\\rhino.jpg";

using var stream = new FileStream(imagePath, FileMode.Open);

var bitmap =  SKBitmap.Decode(stream);

for (int i = 0; i < 4; i++)  
{  
    for (int j = 0; j < 3; j++)  
    {  
        SKRect destRect = new SKRect(j * 50, i * 38, (j + 1) * 50, (i + 1) * 38);  
        canvas.DrawBitmap(bitmap, destRect);  
    }  
}  

var image = surfact.Snapshot();

image.Dump();

