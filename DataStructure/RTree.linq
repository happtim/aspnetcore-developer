<Query Kind="Statements">
  <NuGetReference>RBush</NuGetReference>
  <Namespace>RBush</Namespace>
</Query>


// 创建一个新的 RBush 实例  
var tree = new RBush<Point>();

// 插入一些点  
tree.Insert(new Point(1, 1, "Point A"));
tree.Insert(new Point(2, 2, "Point B"));
tree.Insert(new Point(3, 3, "Point C"));
tree.Insert(new Point(4, 4, "Point D"));
tree.Insert(new Point(5, 5, "Point E"));

Console.WriteLine("Total points: " + tree.Count);

// 搜索一个特定区域内的点  
var searchEnvelope = new Envelope(2, 2, 3, 3);
var results = tree.Search(searchEnvelope);

Console.WriteLine("Points within search area:");
foreach (var point in results)
{
	Console.WriteLine($"- {point.Name} at ({point.X}, {point.Y})");
}

// 删除一个点  
tree.Delete(results.Last());

Console.WriteLine("Total points after deletion: " + tree.Count);

// 清空树  
tree.Clear();

Console.WriteLine("Total points after clearing: " + tree.Count);

// 定义一个简单的点类  
public class Point : ISpatialData
{
	public double X { get; set; }
	public double Y { get; set; }
	public string Name { get; set; }
	
	 private Envelope _envelope;  

	public ref readonly Envelope Envelope => ref _envelope;

	public Point(double x, double y, string name)
	{
		X = x;
		Y = y;
		Name = name;
		_envelope = new Envelope(x, x, y, y); 
	}
}