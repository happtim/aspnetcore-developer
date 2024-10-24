<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

#load "..\Dump"
//剪贴路径就像普通的画布形状，但它充当一个遮罩，隐藏形状的不需要部分。
var imageInfo = new SKImageInfo(300,300);
using var surfact = SKSurface.Create(imageInfo);
var canvas = surfact.Canvas;

// 创建一个矩形  
canvas.DrawRect(0, 0, 150, 150, new SKPaint { Color = SKColors.Black });  

// 平移画布  
canvas.Translate(75, 75);  

// 创建圆形裁剪路径  
using (var clipPath = new SKPath())  
{  
    clipPath.AddCircle(0, 0, 60);  
    canvas.ClipPath(clipPath);  

    // 创建渐变背景  
    using (var gradient = SKShader.CreateLinearGradient(  
        new SKPoint(0, -75),  
        new SKPoint(0, 75),  
        new[] {   
            new SKColor(35, 34, 86), // #232256  
            new SKColor(20, 55, 120)  // #143778  
        },  
        new float[] { 0, 1 },  
        SKShaderTileMode.Clamp))
	{
		using (var paint = new SKPaint { Shader = gradient })
		{
			canvas.DrawRect(-75, -75, 150, 150, paint);
		}
	}

	// 生成星星  
	GenerateStars(canvas);
}

var image = surfact.Snapshot();

image.Dump();



void GenerateStars(SKCanvas canvas)
{
	Random random = new Random();
	for (int j = 1; j < 50; j++)
	{
		canvas.Save();
		float x = 75 - random.Next(150);
		float y = 75 - random.Next(150);
		canvas.Translate(x, y);
		DrawStar(canvas,random.Next(2, 6));
		canvas.Restore();
	}
}

void DrawStar(SKCanvas canvas, float r)
{
	using var path = new SKPath();

	// 定义画笔  
	using var paint = new SKPaint
	{
		Style = SKPaintStyle.Fill,
		Color = SKColors.White,
		IsAntialias = true
	};
		
	double angle = -Math.PI / 2; // 起始角度（-90度，使星星顶点向上）  
	double step = Math.PI / 5;   // 每一步旋转的角度（36度）  

	// 计算五角星的五个顶点  
	for (int i = 0; i < 10; i++)
	{
		// 奇数索引为外顶点，偶数索引为内顶点  
		double currentRadius = (i % 2 == 0) ? (r / 0.525731f) * 0.200811f : r;
		float x = (float)(currentRadius * Math.Cos(angle));
		float y = (float)(currentRadius * Math.Sin(angle));

		if (i == 0)
		{
			path.MoveTo(x, y);
		}
		else
		{
			path.LineTo(x, y);
		}

		angle += step;
	}

	path.Close(); // 关闭路径，形成封闭的五角星  
	canvas.DrawPath(path, paint);
	canvas.Restore();
}