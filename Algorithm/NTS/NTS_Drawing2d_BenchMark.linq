<Query Kind="Program">
  <NuGetReference>BenchmarkDotNet</NuGetReference>
  <NuGetReference>NetTopologySuite</NuGetReference>
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
  <Namespace>BenchmarkDotNet.Running</Namespace>
  <Namespace>NetTopologySuite.Geometries</Namespace>
  <Namespace>System.Drawing.Drawing2D</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

// For industrial-grade benchmarking, add a NuGet reference to BenchmarkDotNet, which has
// special support for LINQPad. The following query compares the hashing performance of
// MD5 vs SHA256. Read more about BenchmarkDotNet here:
//
// https://github.com/dotnet/BenchmarkDotNet/blob/master/README.md

#LINQPad optimize+     // Enable compiler optimizations

void Main()
{
	Util.AutoScrollResults = true;
	BenchmarkRunner.Run<NTSvsDrawing2d>();
}

[ShortRunJob]
public class NTSvsDrawing2d
{
	private readonly Random _random = new Random(42);
	private readonly Polygon _ntsPolygon;
	private readonly int _pointCount = 10000;
	private readonly GraphicsPath _drawingPath;

	public NTSvsDrawing2d()
	{
		// 创建一个复杂的多边形
		Coordinate[] coordinates = new Coordinate[]
		{
			new Coordinate(0, 0),
			new Coordinate(100, 0),
			new Coordinate(100, 100),
			new Coordinate(50, 50),
			new Coordinate(0, 100),
			new Coordinate(0, 0)
		};

		// 为 NTS 创建多边形
		var geometryFactory = new GeometryFactory();
		_ntsPolygon = geometryFactory.CreatePolygon(coordinates);

		// 为 Drawing2D 创建多边形
		PointF[] points = coordinates.Select(c => new PointF((float)c.X, (float)c.Y)).ToArray();
		_drawingPath = new GraphicsPath();
		_drawingPath.AddPolygon(points);

	}

	[Benchmark]
	public int NtsContains()
	{
		int count = 0;
		for (int i = 0; i < _pointCount; i++)
		{
			double x = _random.NextDouble() * 150;
			double y = _random.NextDouble() * 150;
			if (_ntsPolygon.Contains(new NetTopologySuite.Geometries.Point(x, y)))
			{
				count++;
			}
		}
		return count;

	}

	[Benchmark]
	public int Drawing2dContains()
	{
		int count = 0;
		for (int i = 0; i < _pointCount; i++)
		{
			float x = (float)(_random.NextDouble() * 150);
			float y = (float)(_random.NextDouble() * 150);
			if (_drawingPath.IsVisible(x, y))
			{
				count++;
			}
		}
		return count;
	}

}