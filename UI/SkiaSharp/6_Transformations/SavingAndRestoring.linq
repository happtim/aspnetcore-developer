<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"

//Skia 中的 save() 和 restore() 函数是用于管理画布状态的重要方法。这两个函数允许你临时修改画布的状态（如变换、裁剪等），然后在需要时恢复到之前的状态。这对于创建复杂的图形和实现嵌套变换非常有用。
//
//save() 函数:
//
//当你调用 save() 时，它会将当前画布的全部状态保存到一个内部栈中。
//这个状态包括当前的变换矩阵、裁剪区域。
//你可以多次调用 save()，每次调用都会在栈上创建一个新的保存点。
//restore() 函数:
//
//restore() 函数用于恢复到最近一次 save() 时的画布状态。
//它会弹出栈顶的保存状态，并将画布恢复到那个状态。
//每次调用 restore() 都会对应一个之前的 save() 调用。

var imageInfo = new SKImageInfo(300,300);
var surfact = SKSurface.Create(imageInfo);

// 获取 SKCanvas 对象
var canvas = surfact.Canvas;

// 设置背景为白色
canvas.Clear(SkiaSharp.SKColors.White);

using (var paint = new SKPaint()) 
{
	 // 创建矩形  
    var rect = SKRect.Create(0, 0, 25, 25);  

    // 绘制第一个矩形  
    canvas.DrawRect(rect, paint);  

    // 保存画布状态  
    canvas.Save();  

    // 平移画布  
    canvas.Translate(50, 50);  

    // 绘制第二个矩形（平移后的位置）  
    canvas.DrawRect(rect, paint);  

    // 恢复画布状态  
    canvas.Restore();  

    // 设置画笔颜色为红色  
    paint.Color = SKColors.Red;  

    // 绘制第三个矩形（原位置，红色）  
    canvas.DrawRect(rect, paint);  
}


var image = surfact.Snapshot();

image.Dump();

