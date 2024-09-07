<Query Kind="Statements">
  <NuGetReference>NetTopologySuite</NuGetReference>
  <Namespace>NetTopologySuite.Index.KdTree</Namespace>
  <Namespace>NetTopologySuite.Geometries</Namespace>
</Query>


// 创建 KdTree  
var tree = new KdTree<string>();

// 添加点  
tree.Insert(new Coordinate(50.0, 80.0), "100");
tree.Insert(new Coordinate(20.0, 10.0), "201");
tree.Insert(new Coordinate(20.0, 30.1), "202");

// 查找最近邻  
// 创建一个查询点  
Coordinate queryPoint = new Coordinate(20.0, 20.0);

// 查找最近的点  
KdNode<string> nearest = tree.NearestNeighbor(queryPoint);


Console.WriteLine($"查询点: ({queryPoint.X}, {queryPoint.Y})");
Console.WriteLine($"最近的点: ({nearest.Coordinate.X}, {nearest.Coordinate.Y})");

// 查找指定范围内的所有点  
double range = 10;
IList<KdNode<string>> nearbyPoints = tree.Query(new Envelope(queryPoint.X - range, queryPoint.X + range,
															  queryPoint.Y - range, queryPoint.Y + range));

Console.WriteLine($"\n范围 {range} 内的点:");
foreach (var node in nearbyPoints)
{
	Console.WriteLine($"({node.Coordinate.X}, {node.Coordinate.Y})");
}