<Query Kind="Statements">
  <NuGetReference>QuikGraph</NuGetReference>
  <Namespace>QuikGraph</Namespace>
  <Namespace>QuikGraph.Algorithms</Namespace>
</Query>

// 创建一个BidirectionalGraph实例  
// 第一个类型参数是顶点类型，这里使用字符串  
// 第二个类型参数是边的类型，这里使用有向边（DirectedEdge<string>）  
var graph = new BidirectionalGraph<string, Edge<string>>();

// 添加顶点  
graph.AddVertex("A");
graph.AddVertex("B");
graph.AddVertex("C");
graph.AddVertex("D");

// 添加有向边  
graph.AddEdge(new Edge<string>("A", "B"));
graph.AddEdge(new Edge<string>("A", "C"));
graph.AddEdge(new Edge<string>("B", "C"));
graph.AddEdge(new Edge<string>("C", "D"));

// 输出所有顶点  
Console.WriteLine("顶点列表:");
foreach (var vertex in graph.Vertices)
{
	Console.WriteLine(vertex);
}

// 输出所有边及其长度  
Console.WriteLine("\n边列表 (Source -> Target)");
foreach (var edge in graph.Edges)
{
	Console.WriteLine($"{edge.Source} -> {edge.Target} ");
}

