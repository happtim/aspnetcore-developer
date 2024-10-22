<Query Kind="Statements">
  <NuGetReference>NetTopologySuite</NuGetReference>
  <Namespace>NetTopologySuite.Geometries</Namespace>
  <Namespace>NetTopologySuite.Index.Strtree</Namespace>
</Query>

// 创建一个几何工厂  
var geometryFactory = new GeometryFactory();

// 创建一个R树(静态树)
var tree = new STRtree<string>();

// 创建并插入一些几何图形  
var point1 = geometryFactory.CreatePoint(new Coordinate(0, 0));
tree.Insert(point1.EnvelopeInternal, "Point 1");
point1.EnvelopeInternal.Dump();

var point2 = geometryFactory.CreatePoint(new Coordinate(1, 1));
tree.Insert(point2.EnvelopeInternal, "Point 2");
point2.EnvelopeInternal.Dump();

var line = geometryFactory.CreateLineString(new[] { new Coordinate(0, 0), new Coordinate(2, 2) });
tree.Insert(line.EnvelopeInternal, "Line");
line.EnvelopeInternal.Dump();

var polygon = geometryFactory.CreatePolygon(new[]
{
	new Coordinate(3, 3),
	new Coordinate(4, 3),
	new Coordinate(4, 4),
	new Coordinate(3, 4),
	new Coordinate(3, 3)
});
tree.Insert(polygon.EnvelopeInternal, "Polygon");
polygon.EnvelopeInternal.Dump();

// 构建树（在所有插入完成后调用）  
tree.Build();

// 执行空间查询  
var queryEnvelope = new Envelope(0, 2, 0, 2);
var results = tree.Query(queryEnvelope);

Console.WriteLine("Query results:");
foreach (var result in results)
{
	Console.WriteLine(result);
}