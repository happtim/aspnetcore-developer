<Query Kind="Statements">
  <NuGetReference>SkiaSharp</NuGetReference>
  <Namespace>SkiaSharp</Namespace>
</Query>

public static class DumpExtensions
{
	// Move this method into the 'My Extensions' query to make it available to all queries.
	// Notice that we've added a reference to System.Windows.Forms.DataVisualization (press F4).

	public static void Dump(this SKImage image)
	{
		using (var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100))
		{
			// 使用 LINQPad 的 Util 类来显示图像
			// 首先将图像数据转换为字节数组
			var bytes = data.ToArray();
			var ms = new System.IO.MemoryStream(bytes);
			var bitmapImage = new System.Drawing.Bitmap(ms);

			bitmapImage.Dump();
		}
	}
}


public static class SKPathExtensions
{
	//startAngle为零时，起始点位于椭圆的右中边缘
	public static void Arc(this SKPath path, float x, float y, float radius, float startAngle, float endAngle, bool counterclockwise = true)
	{
		//		// Convert angles from radians to degrees
		float startDegrees = startAngle * 180f / (float)Math.PI;
		float endDegrees = endAngle * 180f / (float)Math.PI;

		// SkiaSharp uses sweep angles, not end angles, so calculate the sweep
		float sweepDegrees = endDegrees - startDegrees;

		//负的sweepAngle 逆时针绘制圆弧
		if (counterclockwise)
		{
			if (sweepDegrees > 0)
				sweepDegrees -= 360;
		}

		//正的sweepAngle 顺时针绘制圆弧
		else
		{
			if (sweepDegrees < 0)
				sweepDegrees += 360;
		}

		// SkiaSharp's ArcTo needs the rectangle that bounds the circle
		SKRect rect = new SKRect(x - radius, y - radius, x + radius, y + radius);

		// Add the arc to the path
		path.ArcTo(rect, startDegrees, sweepDegrees, false);
	}
}