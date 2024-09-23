<Query Kind="Statements">
  <NuGetReference>Google.OrTools</NuGetReference>
  <Namespace>Google.OrTools.ConstraintSolver</Namespace>
  <Namespace>Google.Protobuf.WellKnownTypes</Namespace>
</Query>

//https://developers.google.cn/optimization/routing/cvrp?authuser=0

//容量车辆路径问题（CVRP）是一种车辆路径问题，其中具有有限载重能力的车辆需要在各个位置接送物品。
//物品有数量，如重量或体积，车辆有最大容量，它们可以携带。
//问题是以最低成本接送物品，同时永远不超过车辆的容量。

//在之前VRP问题扩展
//在每个位置，都有一个需求，对应于要提取的物品数量。此外，每辆车的最大容量为 15。


// 创建路由模型
// Instantiate the data problem.
DataModel data = new DataModel();

// Create Routing Index Manager
RoutingIndexManager manager =
	new RoutingIndexManager(data.DistanceMatrix.GetLength(0), data.VehicleNumber, data.Depot);

// Create Routing Model.
RoutingModel routing = new RoutingModel(manager);

//定义距离回调
//与 TSP 示例一样，以下函数会创建距离回调，该回调返回位置之间的距离并将其传递给求解器。它还将弧线成本（用于定义行程费用）设置为弧线的距离。

int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
														   {
															   // Convert from routing variable Index to
															   // distance matrix NodeIndex.
															   var fromNode = manager.IndexToNode(fromIndex);
															   var toNode = manager.IndexToNode(toIndex);
															   return data.DistanceMatrix[fromNode, toNode];
														   });
routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

//添加需求回调
//需求回调仅取决于交付的位置（from_node）。
int demandCallbackIndex = routing.RegisterUnaryTransitCallback((long fromIndex) =>
															   {
																   // Convert from routing variable Index to
																   // demand NodeIndex.
																   var fromNode =
																	   manager.IndexToNode(fromIndex);
																   return data.Demands[fromNode];
															   });
															   
//由于容量限制涉及车辆所载货物的重量 — 这是在路线上累积的数量 — 我们需要为容量创建一个维度。
//AddDimensionWithVehicleCapacity 方法添加容量维度，处理更一般的情况，即不同车辆具有不同的容量。
routing.AddDimensionWithVehicleCapacity(demandCallbackIndex, 0, // null capacity slack
										data.VehicleCapacities, // vehicle maximum capacities
										true,                   // start cumul to zero
										"Capacity");

//添加距离维度
//要解决此 VRP 问题，您需要创建一个距离维度，用于计算每辆车沿路线行驶的累计距离。
//然后，您可以设置与每条路线沿途总距离的最大值成比例的费用。路线规划程序使用维度来跟踪车辆行驶路线上累积的数量。
routing.AddDimension(
	transitCallbackIndex, // 过渡回调的索引，用于计算两个节点之间的距离  
	0,                    // slack_max，允许的最大松弛量，这里为0，表示不允许额外的松弛  
	3000,                 // capacity，维度的上限，这里设置每辆车的最大行驶距离为3000  
	true,                 // start_cumul_to_zero，是否将每辆车的起始累积值设为0  
	"Distance"            // 维度的名称  
);
RoutingDimension distanceDimension = routing.GetMutableDimension("Distance");
//方法为路线的全局跨度设置较大的系数 (100)，
//在车辆路径问题（VRP）中，RoutingDimension 通常用于跟踪某些累积量，例如距离、时间或载重。
//全局跨度（Global Span） 是指在所有车辆路径中，某个维度的最大值。这通常用来衡量最耗时或最耗费距离的车辆路线。
//通过将跨度（即所有车辆路径中该维度的最大值）乘以一个系数，并将其加入到优化目标函数中，从而鼓励求解器在优化时尽量减少这个跨度。
distanceDimension.SetGlobalSpanCostCoefficient(100);

// Setting first solution heuristic.
RoutingSearchParameters searchParameters =
	operations_research_constraint_solver.DefaultRoutingSearchParameters();
searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
searchParameters.TimeLimit = new Duration { Seconds = 1 };

// Solve the problem.
Assignment solution = routing.SolveWithParameters(searchParameters);

// Print solution on console.
PrintSolution(data, routing, manager, solution);

//添加解决方案打印机
/// <summary>
///   Print the solution.
/// </summary>
static void PrintSolution(in DataModel data, in RoutingModel routing, in RoutingIndexManager manager,
						  in Assignment solution)
{
	Console.WriteLine($"Objective {solution.ObjectiveValue()}:");

	// Inspect solution.
	long totalDistance = 0;
	long totalLoad = 0;
	for (int i = 0; i < data.VehicleNumber; ++i)
	{
		Console.WriteLine("Route for Vehicle {0}:", i);
		long routeDistance = 0;
		long routeLoad = 0;
		var index = routing.Start(i);
		while (routing.IsEnd(index) == false)
		{
			long nodeIndex = manager.IndexToNode(index);
			routeLoad += data.Demands[nodeIndex];
			Console.Write("{0} Load({1}) -> ", nodeIndex, routeLoad);
			var previousIndex = index;
			index = solution.Value(routing.NextVar(index));
			routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
		}
		Console.WriteLine("{0}", manager.IndexToNode((int)index));
		Console.WriteLine("Distance of the route: {0}m", routeDistance);
		totalDistance += routeDistance;
		totalLoad += routeLoad;
	}
	Console.WriteLine("Total distance of all routes: {0}m", totalDistance);
	Console.WriteLine("Total load of all routes: {0}m", totalLoad);
}


//创建数据
//distance_matrix：一组位置之间的距离（以米为单位）。
//num_vehicles：车队中的车辆数量。
//depot：仓库的索引，这是所有车辆开始和结束路线的位置。
//demands：每个位置都有一个与要提取的物品的数量（例如重量或体积）对应的需求。
//VehicleCapacities：即车辆可以容纳的最大数量。当车辆沿着路线行驶时，它所携带的物品总量永远不会超过其容量。

class DataModel
{
	public long[,] DistanceMatrix = {
		{ 0, 548, 776, 696, 582, 274, 502, 194, 308, 194, 536, 502, 388, 354, 468, 776, 662 },
		{ 548, 0, 684, 308, 194, 502, 730, 354, 696, 742, 1084, 594, 480, 674, 1016, 868, 1210 },
		{ 776, 684, 0, 992, 878, 502, 274, 810, 468, 742, 400, 1278, 1164, 1130, 788, 1552, 754 },
		{ 696, 308, 992, 0, 114, 650, 878, 502, 844, 890, 1232, 514, 628, 822, 1164, 560, 1358 },
		{ 582, 194, 878, 114, 0, 536, 764, 388, 730, 776, 1118, 400, 514, 708, 1050, 674, 1244 },
		{ 274, 502, 502, 650, 536, 0, 228, 308, 194, 240, 582, 776, 662, 628, 514, 1050, 708 },
		{ 502, 730, 274, 878, 764, 228, 0, 536, 194, 468, 354, 1004, 890, 856, 514, 1278, 480 },
		{ 194, 354, 810, 502, 388, 308, 536, 0, 342, 388, 730, 468, 354, 320, 662, 742, 856 },
		{ 308, 696, 468, 844, 730, 194, 194, 342, 0, 274, 388, 810, 696, 662, 320, 1084, 514 },
		{ 194, 742, 742, 890, 776, 240, 468, 388, 274, 0, 342, 536, 422, 388, 274, 810, 468 },
		{ 536, 1084, 400, 1232, 1118, 582, 354, 730, 388, 342, 0, 878, 764, 730, 388, 1152, 354 },
		{ 502, 594, 1278, 514, 400, 776, 1004, 468, 810, 536, 878, 0, 114, 308, 650, 274, 844 },
		{ 388, 480, 1164, 628, 514, 662, 890, 354, 696, 422, 764, 114, 0, 194, 536, 388, 730 },
		{ 354, 674, 1130, 822, 708, 628, 856, 320, 662, 388, 730, 308, 194, 0, 342, 422, 536 },
		{ 468, 1016, 788, 1164, 1050, 514, 514, 662, 320, 274, 388, 650, 536, 342, 0, 764, 194 },
		{ 776, 868, 1552, 560, 674, 1050, 1278, 742, 1084, 810, 1152, 274, 388, 422, 764, 0, 798 },
		{ 662, 1210, 754, 1358, 1244, 708, 480, 856, 514, 468, 354, 844, 730, 536, 194, 798, 0 }
	};
	public int VehicleNumber = 4;
	public int Depot = 0;
	public long[] Demands = { 0, 1, 1, 2, 4, 2, 4, 8, 8, 1, 2, 1, 2, 4, 4, 8, 8 };
	public long[] VehicleCapacities = { 15, 15, 15, 15 };
};