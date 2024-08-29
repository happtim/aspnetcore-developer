<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//矩阵变换（Matrix Transform）是 Skia 中一种强大且灵活的变换方式，它使用 3x3 矩阵来表示和应用各种复杂的二维变换。
//这种方法允许你将多个简单变换组合成一个单一的操作，从而实现更高效和精确的图形变换。

//Skia 使用 SkMatrix 类来表示和操作变换矩阵。这个 3x3 矩阵包含 9 个元素，通常表示为：

//| ScaleX  SkewY TransX |
//| SkewX   ScaleY TransY |
//| Persp0  Persp1 Persp2 |

//ScaleX: X轴缩放因子
//SkewY: Y轴倾斜因子
//TransX: X轴平移距离
//SkewX: X轴倾斜因子
//ScaleY: Y轴缩放因子
//TransY: Y轴平移距离

//在 SkiaSharp 中，你可以使用以下方法来创建或修改 SKMatrix：
//
//创建单位矩阵：SKMatrix.Identity
//创建平移矩阵：SKMatrix.CreateTranslation(tx, ty)
//创建缩放矩阵：SKMatrix.CreateScale(sx, sy)
//创建旋转矩阵：SKMatrix.CreateRotation(angleInRadians)

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 平移画布  
canvas.Translate(100, 100);  

using (var paint = new SKPaint())  
{  
    for (int i = 0; i <= 12; i++)  
    {  
        int c = (int)Math.Floor((255.0 / 12) * i);  
        paint.Color = new SKColor((byte)c, (byte)c, (byte)c);  
        
		//左上（0，0） 右下（100，10）
        canvas.DrawRect(new SKRect(0, 0, 100, 10), paint);  
        
        // 使用矩阵变换代替 transform  
        var matrix = SKMatrix.CreateRotation((float)(Math.PI / 6 ));  
		canvas.Concat(ref matrix);
    }  
}

// 重置并设置新的变换  
var newMatrix = SKMatrix.CreateIdentity();
newMatrix.ScaleX = -1;
newMatrix.TransX = 100;
newMatrix.TransY = 150;
canvas.SetMatrix( newMatrix);

using (var paint = new SKPaint())
{
	paint.Color = new SKColor(255, 128, 255, 128); // 50% 透明度  
	canvas.DrawRect(new SKRect(0, 0, 100, 100), paint);
}

var image = surfact.Snapshot();

image.Dump();

