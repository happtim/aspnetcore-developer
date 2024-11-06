<Query Kind="Statements">
  <NuGetReference>QuikGraph</NuGetReference>
  <Namespace>QuikGraph</Namespace>
  <Namespace>QuikGraph.Algorithms</Namespace>
  <Namespace>QuikGraph.Algorithms.ConnectedComponents</Namespace>
</Query>

// 创建一个有向图  
var graph = new BidirectionalGraph<string, Edge<string>>();

// 添加顶点  
graph.AddVertex("A");
graph.AddVertex("B");
graph.AddVertex("C");
graph.AddVertex("D");

// 添加边 (有向)  
graph.AddEdge(new Edge<string>("A", "B"));
graph.AddEdge(new Edge<string>("B", "C"));
graph.AddEdge(new Edge<string>("C", "A"));
graph.AddEdge(new Edge<string>("C", "D"));

// 创建 StronglyConnectedComponentsAlgorithm  
var sccAlgorithm = new StronglyConnectedComponentsAlgorithm<string, Edge<string>>(graph);

// 执行算法  
sccAlgorithm.Compute();
var componentMap = sccAlgorithm.Components;

// 输出强连通分量  
Console.WriteLine("强连通分量：");
foreach (var kvp in componentMap)
{
	Console.WriteLine($"顶点 {kvp.Key} 属于分量 {kvp.Value}");
}

// 检查是否只有一个强连通分量  
int numberOfComponents = new HashSet<int>(componentMap.Values).Count;
if (numberOfComponents == 1)
{
	Console.WriteLine("该图是强连通的。");
}
else
{
	Console.WriteLine("该图不是强连通的。");
}
