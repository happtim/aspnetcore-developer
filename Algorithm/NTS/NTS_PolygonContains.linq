<Query Kind="Statements">
  <NuGetReference>NetTopologySuite</NuGetReference>
  <Namespace>NetTopologySuite.Geometries</Namespace>
</Query>

// 创建一个 GeometryFactory
var geometryFactory = new GeometryFactory();

// 创建一个多边形
Coordinate[] coordinates = new Coordinate[]
{
	new Coordinate(0, 0),
	new Coordinate(10, 0),
	new Coordinate(10, 10),
	new Coordinate(0, 10),
	new Coordinate(0, 0)  // 闭合多边形
};

var polygon = geometryFactory.CreatePolygon(coordinates);

// 创建要检查的点
var point1 = geometryFactory.CreatePoint(new Coordinate(5, 5));
var point2 = geometryFactory.CreatePoint(new Coordinate(15, 15));

// 检查点是否在多边形内
bool isInside1 = polygon.Contains(point1);
bool isInside2 = polygon.Contains(point2);

Console.WriteLine($"Point (5, 5) is inside the polygon: {isInside1}");
Console.WriteLine($"Point (15, 15) is inside the polygon: {isInside2}");