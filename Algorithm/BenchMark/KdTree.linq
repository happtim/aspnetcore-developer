<Query Kind="Program">
  <NuGetReference>BenchmarkDotNet</NuGetReference>
  <NuGetReference>KdTree</NuGetReference>
  <NuGetReference>NetTopologySuite</NuGetReference>
  <Namespace>KdTree</Namespace>
  <Namespace>KdTree.Math</Namespace>
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
  <Namespace>NetTopologySuite.Geometries</Namespace>
  <Namespace>BenchmarkDotNet.Running</Namespace>
</Query>

#LINQPad optimize+     // Enable compiler optimizations

void Main()
{
	
	Util.AutoScrollResults = true;
	var summary = BenchmarkRunner.Run<NearestStationBenchmark>();
}

[MemoryDiagnoser]
[SimpleJob]
public class NearestStationBenchmark
{
	private List<Position> _positions = new List<Position>();
	private NetTopologySuite.Index.KdTree.KdTree<Position> _ntsKdTree;
	private KdTree<float, Position> _kdTreeLib ;
	private readonly Random _random = new Random(42); // 固定种子确保可重现

	// 查询点
	private float _queryX;
	private float _queryY;
	private const float SearchRange = 0.6f; // 0.6米范围
	[Params(100, 1000, 5000)] // 测试不同数据量
	public int StationCount { get; set; }
	[GlobalSetup]
	public void Setup()
	{
		Console.WriteLine($"设置 {StationCount} 个站点的测试数据...");

		// 清理之前的数据
		_positions.Clear();
		_ntsKdTree = new NetTopologySuite.Index.KdTree.KdTree<Position>();
		_kdTreeLib = new KdTree<float, Position>(2, new FloatMath());
		// 生成随机站点
		for (int i = 0; i < StationCount; i++)
		{
			float x = (float)(_random.NextDouble() * 10000);
			float y = (float)(_random.NextDouble() * 10000);
			var position = new Position(x, y);

			_positions.Add(position);

			// 添加到 NetTopologySuite KdTree
			_ntsKdTree.Insert(new Coordinate(x, y), position);

			// 添加到 KdTree 库
			_kdTreeLib.Add(new[] { x, y }, position);
		}
		
		// 设置查询点（在数据范围中间，增加找到邻近点的概率）
		_queryX = 5000f;
		_queryY = 5000f;	
		Console.WriteLine($"测试数据准备完成。查询点: ({_queryX}, {_queryY})，搜索范围: {SearchRange}");
	}
	[Benchmark(Baseline = true)]
	public List<Position> Method1_Lambda()
	{
		return _positions
			.Where(p => Math.Sqrt((p.Y - _queryY) * (p.Y - _queryY) + (p.X - _queryX) * (p.X - _queryX)) <= SearchRange)
			.ToList();
	}
	[Benchmark]
	public List<Position> NetTopologySuite_KdTree()
	{
		// 使用 Envelope 进行范围查询
		var envelope = new Envelope(
			_queryX - SearchRange, _queryX + SearchRange,
			_queryY - SearchRange, _queryY + SearchRange);

		var candidates = _ntsKdTree.Query(envelope);

		// 进一步筛选精确距离（因为 Envelope 是矩形，我们需要圆形范围）
		var result = new List<Position>();
		foreach (var node in candidates)
		{
			var distance = Math.Sqrt(
				(node.Coordinate.X - _queryX) * (node.Coordinate.X - _queryX) +
				(node.Coordinate.Y - _queryY) * (node.Coordinate.Y - _queryY));

			if (distance <= SearchRange)
			{
				result.Add(node.Data);
			}
		}

		return result;
	}
	[Benchmark]
	public List<Position> KdTree_Library()
	{
		// KdTree 库没有直接的范围查询，我们使用最近邻查询然后筛选

		var nearestNodes = _kdTreeLib.GetNearestNeighbours(new[] { _queryX, _queryY }, 3);

		var result = new List<Position>();
		foreach (var node in nearestNodes)
		{
			var distance = Math.Sqrt(
				(node.Point[0] - _queryX) * (node.Point[0] - _queryX) +
				(node.Point[1] - _queryY) * (node.Point[1] - _queryY));

			if (distance <= SearchRange)
			{
				result.Add(node.Value);
			}
			else
			{
				// 由于结果是按距离排序的，一旦超出范围就可以停止
				break;
			}
		}

		return result;
	}
}


	// 基础数据结构
	public class Position
	{
		public float X { get; set; }
		public float Y { get; set; }
		public Position(float x, float y)
	{
		X = x;
		Y = y;
	}
	public override string ToString()
	{
		return $"({X}, {Y})";
	}
}
