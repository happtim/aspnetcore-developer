<Query Kind="Statements">
  <NuGetReference>QuikGraph</NuGetReference>
  <Namespace>QuikGraph</Namespace>
  <Namespace>QuikGraph.Algorithms</Namespace>
</Query>

// 主程序  
var graph = new BidirectionalGraph<Vertex, WeightedEdge>();

// 创建带坐标的顶点  
var vertexA = new Vertex("A", 0, 0);
var vertexB = new Vertex("B", 1, 1);
var vertexC = new Vertex("C", 2, -1);
var vertexD = new Vertex("D", 3, 2);

// 添加顶点  
graph.AddVertex(vertexA);
graph.AddVertex(vertexB);
graph.AddVertex(vertexC);
graph.AddVertex(vertexD);

// 添加边  
graph.AddEdge(new WeightedEdge(vertexA, vertexB));
graph.AddEdge(new WeightedEdge(vertexA, vertexC));
graph.AddEdge(new WeightedEdge(vertexB, vertexC));
graph.AddEdge(new WeightedEdge(vertexC, vertexD));

// 输出所有顶点  
Console.WriteLine("顶点列表:");
foreach (var vertex in graph.Vertices)
{
	Console.WriteLine(vertex.ToString());
}

// 输出所有边及其长度  
Console.WriteLine("\n边列表 (Source -> Target : Length):");
foreach (var edge in graph.Edges)
{
	Console.WriteLine($"{edge.Source} -> {edge.Target} : {edge.Weight}");
}

// 定义边的权重函数  
Func<WeightedEdge, double> edgeWeights = edge => edge.Weight;

// 定义使用曼哈顿距离的启发式函数  
Func<Vertex, double> heuristic = vertex =>
{
	// 计算到目标顶点D的曼哈顿距离  
	return Math.Abs(vertex.X - vertexD.X) + Math.Abs(vertex.Y - vertexD.Y);
};

// 使用A*算法找最短路径  
var tryGetPath = graph.ShortestPathsAStar(edgeWeights, heuristic, vertexA);

// 获取从A到D的路径  
if (tryGetPath(vertexD, out IEnumerable<WeightedEdge> path))
{
	Console.WriteLine("\n从A到D的最短路径:");
	double totalWeight = 0;
	string pathString = vertexA.Name;

	foreach (var edge in path)
	{
		pathString += " -> " + edge.Target.Name;
		totalWeight += edge.Weight;
	}

	Console.WriteLine(pathString);
	Console.WriteLine($"总距离: {totalWeight}");
}
else
{
	Console.WriteLine("\n没有找到从A到D的路径");
}

// 定义带权重的边类  
public class WeightedEdge : Edge<Vertex>
{
	public double Weight { get; set; }

	public WeightedEdge(Vertex source, Vertex target)
		: base(source, target)
	{
		Weight = CalculateDistance(source, target);
	}

	// 计算曼哈顿距离或欧几里得距离  
	private double CalculateDistance(Vertex v1, Vertex v2)
	{
		// 如果需要使用欧几里得距离，可以使用以下代码：  
		 return Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));  
	}
}

// 修改顶点类以包含坐标信息  
public class Vertex
{
	public string Name { get; set; }
	public double X { get; set; }
	public double Y { get; set; }

	public Vertex(string name, double x, double y)
	{
		Name = name;
		X = x;
		Y = y;
	}

	public override string ToString()
	{
		return Name;
	}
}
