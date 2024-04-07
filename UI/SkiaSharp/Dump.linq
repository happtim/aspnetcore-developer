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