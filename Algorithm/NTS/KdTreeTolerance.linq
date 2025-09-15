<Query Kind="Statements">
  <NuGetReference>NetTopologySuite</NuGetReference>
  <Namespace>NetTopologySuite.Index.KdTree</Namespace>
  <Namespace>NetTopologySuite.Geometries</Namespace>
</Query>


// 创建 KdTree  
var tree = new KdTree<string>();

// 示例1：tolerance = 0 (严格模式)
var strictTree = new KdTree<string>(0.0);
strictTree.Insert(new Coordinate(10.0, 20.0), "Point A");
strictTree.Insert(new Coordinate(10.1, 20.1), "Point B");  // 距离 ≈ 0.14，会被插入
strictTree.Insert(new Coordinate(10.0, 20.0), "Point C");  // 完全相同坐标，会被合并到 Point A
Console.WriteLine($"严格模式树中的点数: {strictTree.Count}");  // 输出: 2
foreach (var node in strictTree.Query(new Envelope(9, 11, 19, 21)))
{
	Console.WriteLine($"坐标: ({node.Coordinate.X}, {node.Coordinate.Y}), 数据: {node.Data}");
}
"".Dump();

// 示例2：tolerance = 0.5 (宽松模式)
var tolerantTree = new KdTree<string>(0.5);
tolerantTree.Insert(new Coordinate(10.0, 20.0), "Point A");
tolerantTree.Insert(new Coordinate(10.1, 20.1), "Point B");  // 距离 ≈ 0.14 < 0.5，会被合并到 Point A
tolerantTree.Insert(new Coordinate(10.6, 20.0), "Point C");  // 距离 = 0.6 > 0.5，会被插入
Console.WriteLine($"宽松模式树中的点数: {tolerantTree.Count}");  // 输出: 2
foreach (var node in tolerantTree.Query(new Envelope(9, 11, 19, 21)))
{
	Console.WriteLine($"坐标: ({node.Coordinate.X}, {node.Coordinate.Y}), 数据: {node.Data}");
}