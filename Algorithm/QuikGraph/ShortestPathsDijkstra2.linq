<Query Kind="Statements">
  <NuGetReference>QuikGraph</NuGetReference>
  <Namespace>QuikGraph</Namespace>
  <Namespace>QuikGraph.Algorithms</Namespace>
</Query>

// 创建一个BidirectionalGraph实例  
// 第一个类型参数是顶点类型，这里使用字符串  
// 第二个类型参数是边的类型，这里使用有向边（DirectedEdge<string>）  
var graph = new BidirectionalGraph<string, WeightedEdge>();

// 添加顶点  
graph.AddVertex("A");
graph.AddVertex("B");
graph.AddVertex("C");
graph.AddVertex("D");

// 添加有向边  
graph.AddEdge(new WeightedEdge("A", "B", 5.0f));
graph.AddEdge(new WeightedEdge("A", "C", 3.0f));
graph.AddEdge(new WeightedEdge("B", "C", 2.0f));
graph.AddEdge(new WeightedEdge("C", "D", 7.0f));

// 输出所有顶点  
Console.WriteLine("顶点列表:");
foreach (var vertex in graph.Vertices)
{
	Console.WriteLine(vertex);
}

// 输出所有边及其长度  
Console.WriteLine("\n边列表 (Source -> Target : Length):");
foreach (var edge in graph.Edges)
{
	Console.WriteLine($"{edge.Source} -> {edge.Target} : {edge.Weight}");
}

// 使用 Dijkstra 算法查找最短路径  
var tryFindPath = graph.ShortestPathsDijkstra(e => e.Weight, "A");
bool hasPath = tryFindPath("D", out IEnumerable<WeightedEdge> path);
if (hasPath)
{
	Console.WriteLine("\n从A到D的最短路径:");
	double totalLength = 0;
	foreach (var edge in path)
	{
		Console.WriteLine($"{edge.Source} -> {edge.Target} : {edge.Weight}");
		totalLength += edge.Weight;
	}
	Console.WriteLine($"总长度: {totalLength}");
}
else
{
	Console.WriteLine("\n没有找到从A到D的路径。");
}

// 自定义带权重的边类  
public class WeightedEdge : Edge<string>
{
	public double Weight { get; set; }

	public WeightedEdge(string source, string target, double weight)
		: base(source, target)
	{
		Weight = weight;
	}
}
