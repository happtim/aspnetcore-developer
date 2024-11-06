<Query Kind="Statements">
  <NuGetReference>QuikGraph</NuGetReference>
  <Namespace>QuikGraph</Namespace>
  <Namespace>QuikGraph.Algorithms</Namespace>
</Query>

// 创建一个BidirectionalGraph实例  
// 第一个类型参数是顶点类型，这里使用字符串  
// 第二个类型参数是边的类型，这里使用有向边（DirectedEdge<string>）  
var graph = new BidirectionalGraph<string, TaggedEdge<string, float>>();

// 添加顶点  
graph.AddVertex("A");
graph.AddVertex("B");
graph.AddVertex("C");
graph.AddVertex("D");

// 添加有向边  
graph.AddEdge(new TaggedEdge<string, float>("A", "B", 5.0f));
graph.AddEdge(new TaggedEdge<string, float>("A", "C", 3.0f));
graph.AddEdge(new TaggedEdge<string, float>("B", "C", 2.0f));
graph.AddEdge(new TaggedEdge<string, float>("C", "D", 7.0f));

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
	Console.WriteLine($"{edge.Source} -> {edge.Target} : {edge.Tag}");
}

// 示例：查找从A到D的最短路径（基于边的长度）  
// 定义权重选择器，即每条边的权重为其长度  
Func<TaggedEdge<string, float>, double> edgeCost = e => e.Tag;

// 使用 Dijkstra 算法查找最短路径  
var tryFindPath = graph.ShortestPathsDijkstra(edgeCost, "A");
bool hasPath = tryFindPath("D", out IEnumerable<TaggedEdge<string, float>> path);
if (hasPath)
{
	Console.WriteLine("\n从A到D的最短路径:");
	double totalLength = 0;
	foreach (var edge in path)
	{
		Console.WriteLine($"{edge.Source} -> {edge.Target} : {edge.Tag}");
		totalLength += edge.Tag;
	}
	Console.WriteLine($"总长度: {totalLength}");
}
else
{
	Console.WriteLine("\n没有找到从A到D的路径。");
}
