<Query Kind="Statements">
  <NuGetReference>Google.OrTools</NuGetReference>
  <Namespace>Google.OrTools.ConstraintSolver</Namespace>
</Query>

//https://developers.google.cn/optimization/routing/tsp?authuser=0&hl=zh-cn#circuit_board

//下一个示例涉及使用自动钻孔在电路板上钻孔。但问题在于，需要找到钻孔在板上要走的最短路线，以便钻孔所需的全部孔。

#load ".\Output"

//创建路由模型
DataModel data = new DataModel();
RoutingIndexManager manager =
	new RoutingIndexManager(data.Locations.GetLength(0), data.VehicleNumber, data.Depot);
RoutingModel routing = new RoutingModel(manager);


//添加距离回调
long[,] distanceMatrix = ComputeEuclideanDistanceMatrix(data.Locations);
int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
														   {
															   // Convert from routing variable Index to
															   // distance matrix NodeIndex.
															   var fromNode = manager.IndexToNode(fromIndex);
															   var toNode = manager.IndexToNode(toIndex);
															   return distanceMatrix[fromNode, toNode];
														   });
routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

//设置搜索参数
// Setting first solution heuristic.
RoutingSearchParameters searchParameters =
	operations_research_constraint_solver.DefaultRoutingSearchParameters();
searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;

//求解并打印解决方案
// Solve the problem.
Assignment solution = routing.SolveWithParameters(searchParameters);

// Print solution on console.
TSPPrintSolution(routing, manager, solution);

//计算距离矩阵
//下面的函数会计算数据中任意两点之间的欧几里得距离，并将其存储在数组中。由于路由求解器适用于整数，因此该函数会将计算的距离四舍五入为整数。

/// <summary>
///   Euclidean distance implemented as a callback. It uses an array of
///   positions and computes the Euclidean distance between the two
///   positions of two different indices.
/// </summary>
static long[,] ComputeEuclideanDistanceMatrix(in int[,] locations)
{
	// Calculate the distance matrix using Euclidean distance.
	int locationNumber = locations.GetLength(0);
	long[,] distanceMatrix = new long[locationNumber, locationNumber];
	for (int fromNode = 0; fromNode < locationNumber; fromNode++)
	{
		for (int toNode = 0; toNode < locationNumber; toNode++)
		{
			if (fromNode == toNode)
				distanceMatrix[fromNode, toNode] = 0;
			else
				distanceMatrix[fromNode, toNode] =
					(long)Math.Sqrt(Math.Pow(locations[toNode, 0] - locations[fromNode, 0], 2) +
								Math.Pow(locations[toNode, 1] - locations[fromNode, 1], 2));
		}
	}
	return distanceMatrix;
}

//创建数据
//该问题的数据包括平面中的 280 个点，如上面的散点图所示。该程序会以与平面中的点对应的有序对数组的形式创建数据，

class DataModel
{
	public int[,] Locations = {
		{ 288, 149 }, { 288, 129 }, { 270, 133 }, { 256, 141 }, { 256, 157 }, { 246, 157 }, { 236, 169 },
		{ 228, 169 }, { 228, 161 }, { 220, 169 }, { 212, 169 }, { 204, 169 }, { 196, 169 }, { 188, 169 },
		{ 196, 161 }, { 188, 145 }, { 172, 145 }, { 164, 145 }, { 156, 145 }, { 148, 145 }, { 140, 145 },
		{ 148, 169 }, { 164, 169 }, { 172, 169 }, { 156, 169 }, { 140, 169 }, { 132, 169 }, { 124, 169 },
		{ 116, 161 }, { 104, 153 }, { 104, 161 }, { 104, 169 }, { 90, 165 },  { 80, 157 },  { 64, 157 },
		{ 64, 165 },  { 56, 169 },  { 56, 161 },  { 56, 153 },  { 56, 145 },  { 56, 137 },  { 56, 129 },
		{ 56, 121 },  { 40, 121 },  { 40, 129 },  { 40, 137 },  { 40, 145 },  { 40, 153 },  { 40, 161 },
		{ 40, 169 },  { 32, 169 },  { 32, 161 },  { 32, 153 },  { 32, 145 },  { 32, 137 },  { 32, 129 },
		{ 32, 121 },  { 32, 113 },  { 40, 113 },  { 56, 113 },  { 56, 105 },  { 48, 99 },   { 40, 99 },
		{ 32, 97 },   { 32, 89 },   { 24, 89 },   { 16, 97 },   { 16, 109 },  { 8, 109 },   { 8, 97 },
		{ 8, 89 },    { 8, 81 },    { 8, 73 },    { 8, 65 },    { 8, 57 },    { 16, 57 },   { 8, 49 },
		{ 8, 41 },    { 24, 45 },   { 32, 41 },   { 32, 49 },   { 32, 57 },   { 32, 65 },   { 32, 73 },
		{ 32, 81 },   { 40, 83 },   { 40, 73 },   { 40, 63 },   { 40, 51 },   { 44, 43 },   { 44, 35 },
		{ 44, 27 },   { 32, 25 },   { 24, 25 },   { 16, 25 },   { 16, 17 },   { 24, 17 },   { 32, 17 },
		{ 44, 11 },   { 56, 9 },    { 56, 17 },   { 56, 25 },   { 56, 33 },   { 56, 41 },   { 64, 41 },
		{ 72, 41 },   { 72, 49 },   { 56, 49 },   { 48, 51 },   { 56, 57 },   { 56, 65 },   { 48, 63 },
		{ 48, 73 },   { 56, 73 },   { 56, 81 },   { 48, 83 },   { 56, 89 },   { 56, 97 },   { 104, 97 },
		{ 104, 105 }, { 104, 113 }, { 104, 121 }, { 104, 129 }, { 104, 137 }, { 104, 145 }, { 116, 145 },
		{ 124, 145 }, { 132, 145 }, { 132, 137 }, { 140, 137 }, { 148, 137 }, { 156, 137 }, { 164, 137 },
		{ 172, 125 }, { 172, 117 }, { 172, 109 }, { 172, 101 }, { 172, 93 },  { 172, 85 },  { 180, 85 },
		{ 180, 77 },  { 180, 69 },  { 180, 61 },  { 180, 53 },  { 172, 53 },  { 172, 61 },  { 172, 69 },
		{ 172, 77 },  { 164, 81 },  { 148, 85 },  { 124, 85 },  { 124, 93 },  { 124, 109 }, { 124, 125 },
		{ 124, 117 }, { 124, 101 }, { 104, 89 },  { 104, 81 },  { 104, 73 },  { 104, 65 },  { 104, 49 },
		{ 104, 41 },  { 104, 33 },  { 104, 25 },  { 104, 17 },  { 92, 9 },    { 80, 9 },    { 72, 9 },
		{ 64, 21 },   { 72, 25 },   { 80, 25 },   { 80, 25 },   { 80, 41 },   { 88, 49 },   { 104, 57 },
		{ 124, 69 },  { 124, 77 },  { 132, 81 },  { 140, 65 },  { 132, 61 },  { 124, 61 },  { 124, 53 },
		{ 124, 45 },  { 124, 37 },  { 124, 29 },  { 132, 21 },  { 124, 21 },  { 120, 9 },   { 128, 9 },
		{ 136, 9 },   { 148, 9 },   { 162, 9 },   { 156, 25 },  { 172, 21 },  { 180, 21 },  { 180, 29 },
		{ 172, 29 },  { 172, 37 },  { 172, 45 },  { 180, 45 },  { 180, 37 },  { 188, 41 },  { 196, 49 },
		{ 204, 57 },  { 212, 65 },  { 220, 73 },  { 228, 69 },  { 228, 77 },  { 236, 77 },  { 236, 69 },
		{ 236, 61 },  { 228, 61 },  { 228, 53 },  { 236, 53 },  { 236, 45 },  { 228, 45 },  { 228, 37 },
		{ 236, 37 },  { 236, 29 },  { 228, 29 },  { 228, 21 },  { 236, 21 },  { 252, 21 },  { 260, 29 },
		{ 260, 37 },  { 260, 45 },  { 260, 53 },  { 260, 61 },  { 260, 69 },  { 260, 77 },  { 276, 77 },
		{ 276, 69 },  { 276, 61 },  { 276, 53 },  { 284, 53 },  { 284, 61 },  { 284, 69 },  { 284, 77 },
		{ 284, 85 },  { 284, 93 },  { 284, 101 }, { 288, 109 }, { 280, 109 }, { 276, 101 }, { 276, 93 },
		{ 276, 85 },  { 268, 97 },  { 260, 109 }, { 252, 101 }, { 260, 93 },  { 260, 85 },  { 236, 85 },
		{ 228, 85 },  { 228, 93 },  { 236, 93 },  { 236, 101 }, { 228, 101 }, { 228, 109 }, { 228, 117 },
		{ 228, 125 }, { 220, 125 }, { 212, 117 }, { 204, 109 }, { 196, 101 }, { 188, 93 },  { 180, 93 },
		{ 180, 101 }, { 180, 109 }, { 180, 117 }, { 180, 125 }, { 196, 145 }, { 204, 145 }, { 212, 145 },
		{ 220, 145 }, { 228, 145 }, { 236, 145 }, { 246, 141 }, { 252, 125 }, { 260, 129 }, { 280, 133 },
	};
	public int VehicleNumber = 1;
	public int Depot = 0;
};