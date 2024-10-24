<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
  <RemoveNamespace>System.Threading</RemoveNamespace>
</Query>

#load "..\Dump"

// 设置定时器每秒刷新  
System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
timer.Interval = 1000; // 1秒  
timer.Tick += (s, e) => DrawClock() ;
timer.Start();

void DrawClock()
{
	var imageInfo = new SKImageInfo(300, 300);
	using var surfact = SKSurface.Create(imageInfo);
	var canvas = surfact.Canvas;

	// 移动坐标系到中心点  
	canvas.Translate(300 / 2, 300 / 2);

	// 缩放  
	float scale = Math.Min(300, 300) * 0.4f;
	canvas.Scale(scale);

	// 旋转坐标系使12点朝上  
	canvas.RotateDegrees(-90);

	using (var paint = new SKPaint
	{
		Style = SKPaintStyle.Stroke,
		Color = SKColors.Black,
		StrokeWidth = 0.08f,
		StrokeCap = SKStrokeCap.Round,
		IsAntialias = true
	})
	{
		// 绘制小时刻度  
		for (int i = 0; i < 12; i++)
		{
			canvas.Save();
			canvas.RotateDegrees(i * 30);
			canvas.DrawLine(new SKPoint(0.8f, 0), new SKPoint(1.0f, 0), paint);
			canvas.Restore();
		}

		// 绘制分钟刻度  
		paint.StrokeWidth = 0.05f;
		for (int i = 0; i < 60; i++)
		{
			if (i % 5 != 0)
			{
				canvas.Save();
				canvas.RotateDegrees(i * 6);
				canvas.DrawLine(new SKPoint(0.9f, 0), new SKPoint(1.0f, 0), paint);
				canvas.Restore();
			}
		}

		// 获取当前时间  
		var now = DateTime.Now;
		float hour = now.Hour % 12 + now.Minute / 60.0f;
		float minute = now.Minute + now.Second / 60.0f;
		float second = now.Second;

		// 绘制时针  
		paint.StrokeWidth = 0.14f;
		canvas.Save();
		canvas.RotateDegrees(hour * 30);
		canvas.DrawLine(new SKPoint(-0.2f, 0), new SKPoint(0.6f, 0), paint);
		canvas.Restore();

		// 绘制分针  
		paint.StrokeWidth = 0.1f;
		canvas.Save();
		canvas.RotateDegrees(minute * 6);
		canvas.DrawLine(new SKPoint(-0.28f, 0), new SKPoint(0.8f, 0), paint);
		canvas.Restore();

		// 绘制秒针  
		paint.Color = SKColors.Red;
		paint.StrokeWidth = 0.06f;
		canvas.Save();
		canvas.RotateDegrees(second * 6);
		canvas.DrawLine(new SKPoint(-0.3f, 0), new SKPoint(0.83f, 0), paint);

		// 绘制秒针中心点和尾部圆点  
		using (var circlePaint = new SKPaint
		{
			Style = SKPaintStyle.Fill,
			Color = SKColors.Red,
			IsAntialias = true
		})
		{
			canvas.DrawCircle(0, 0, 0.1f, circlePaint);
			canvas.DrawCircle(0.95f, 0, 0.1f, circlePaint);
		}
		canvas.Restore();

		// 绘制外圆  
		paint.Color = new SKColor(50, 95, 162); // #325FA2  
		paint.StrokeWidth = 0.14f;
		canvas.DrawCircle(0, 0, 1.1f, paint);
	}
	var image = surfact.Snapshot();
	
	Util.ClearResults();  
	image.Dump();
}



