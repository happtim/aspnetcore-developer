<Query Kind="Statements">
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Drawing2D</Namespace>
</Query>

// 定义多边形的点
PointF[] polygonPoints = new PointF[]
{
	new PointF(0, 0),
	new PointF(100, 0),
	new PointF(100, 100),
	new PointF(0, 100)
};

// 创建 GraphicsPath
using (GraphicsPath path = new GraphicsPath())
{
	path.AddPolygon(polygonPoints);

	// 定义要检查的点
	PointF pointInside = new PointF(50, 50);
	PointF pointOutside = new PointF(150, 150);

	// 检查点是否在多边形内
	bool isInside1 = path.IsVisible(pointInside);
	bool isInside2 = path.IsVisible(pointOutside);

	Console.WriteLine($"Point (50, 50) is inside the polygon: {isInside1}");
	Console.WriteLine($"Point (150, 150) is inside the polygon: {isInside2}");
}