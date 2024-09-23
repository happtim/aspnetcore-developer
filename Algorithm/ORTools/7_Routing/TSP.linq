<Query Kind="Statements">
  <NuGetReference>Google.OrTools</NuGetReference>
  <Namespace>Google.OrTools.ConstraintSolver</Namespace>
</Query>

//https://developers.google.cn/optimization/routing/tsp?authuser=0&hl=zh-cn

//最常见的优化任务之一是车辆路线，其目标是为前往一组地点的车队找到最佳路线。通常，“最佳”是指总距离或费用最少的路线。 
//以下是路由问题的一些示例：
//一家包裹配送服务公司想要为司机指定送货路线。
//一家有线电视公司希望为技术人员分配用于拨打住宅服务电话的路由。
//一家拼车公司想要为司机分配上车和下车路线。
//其中最著名的路线问题是旅行推销员问题(TSP)：对于需要探访不同营业地点的客户并返回出发地的销售人员，找出最短的路线。

//地点越多，问题就越复杂，如果有 10 个位置（不计算起点），则路由数量为 362880。如果是 20 个营业地点，此数量会跳到 2432902008176640000。

//更通用的 TSP 版本是车辆路线问题 (VRP)，其中有多辆车辆。
//在大多数情况下，VRP 都有限制：例如，车辆可能具有承载最大重量或最大体积的物品容量，
//或者驾驶员可能需要在客户要求的指定时间范围内造访某些地点。

//车辆路线问题从本质上说是难以解决的：解决问题所需的时间会随着问题规模成倍增长。
//对于足够大的问题，OR-Tools（或任何其他路由软件）可能需要数年时间才能找到最佳解决方案。
//因此，OR-Tools 有时会返回良好的解决方案，但并非最佳解决方案。如需找到更好的解决方案，请更改求解器的搜索选项。

#load ".\Output"

//TSP Sample 创建路由模型

DataModel data = new DataModel();
RoutingIndexManager manager =
//参数1：距离矩阵的行数，即位置数量（包括车站）。
//参数2：出现问题的车辆数量。
//参数3：与仓库对应的节点。
	new RoutingIndexManager(data.DistanceMatrix.GetLength(0), data.VehicleNumber, data.Depot);
RoutingModel routing = new RoutingModel(manager);

//创建距离回调
//如需使用路由求解器，您需要创建一个距离（或公交）回调：该函数可接受任意一对位置并返回它们之间的距离。
//最简单的方法是使用距离矩阵。

int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
														   {
															   // Convert from routing variable Index to
															   // distance matrix NodeIndex.
															   var fromNode = manager.IndexToNode(fromIndex);
															   var toNode = manager.IndexToNode(toIndex);
															   return data.DistanceMatrix[fromNode, toNode];
														   });
//设置旅行费用
//弧成本评估器告诉求解器如何计算任意两个位置之间的旅行成本。
//换句话说，问题图中连接它们的边缘（或弧）的成本。以下代码设置了弧成本评估器。
routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

//设置搜索参数
//以下代码会设置默认搜索参数和用于查找第一个解决方案的启发法：
//上述代码会将第一个解决方案策略设置为 PATH_CHEAPEST_ARC，它会通过添加权重最低且不指向之前访问过的节点（仓库除外）的边缘来为求解器创建初始路线。
RoutingSearchParameters searchParameters =
	operations_research_constraint_solver.DefaultRoutingSearchParameters();
searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;

//求解并打印解决方案
Assignment solution = routing.SolveWithParameters(searchParameters);
TSPPrintSolution(routing, manager, solution);


//距离矩阵是一个数组，其 i、j 条目是从位置 i 到位置 j（以英里为单位）的距离，其中数组索引与位置对应的顺序如下：

//0. New York - 1. Los Angeles - 2. Chicago - 3. Minneapolis - 4. Denver - 5. Dallas
//-6.Seattle - 7.Boston - 8.San Francisco - 9.St.Louis - 10.Houston - 11.Phoenix - 12.Salt Lake City
class DataModel
{
	public long[,] DistanceMatrix = {
		{ 0, 2451, 713, 1018, 1631, 1374, 2408, 213, 2571, 875, 1420, 2145, 1972 },
		{ 2451, 0, 1745, 1524, 831, 1240, 959, 2596, 403, 1589, 1374, 357, 579 },
		{ 713, 1745, 0, 355, 920, 803, 1737, 851, 1858, 262, 940, 1453, 1260 },
		{ 1018, 1524, 355, 0, 700, 862, 1395, 1123, 1584, 466, 1056, 1280, 987 },
		{ 1631, 831, 920, 700, 0, 663, 1021, 1769, 949, 796, 879, 586, 371 },
		{ 1374, 1240, 803, 862, 663, 0, 1681, 1551, 1765, 547, 225, 887, 999 },
		{ 2408, 959, 1737, 1395, 1021, 1681, 0, 2493, 678, 1724, 1891, 1114, 701 },
		{ 213, 2596, 851, 1123, 1769, 1551, 2493, 0, 2699, 1038, 1605, 2300, 2099 },
		{ 2571, 403, 1858, 1584, 949, 1765, 678, 2699, 0, 1744, 1645, 653, 600 },
		{ 875, 1589, 262, 466, 796, 547, 1724, 1038, 1744, 0, 679, 1272, 1162 },
		{ 1420, 1374, 940, 1056, 879, 225, 1891, 1605, 1645, 679, 0, 1017, 1200 },
		{ 2145, 357, 1453, 1280, 586, 887, 1114, 2300, 653, 1272, 1017, 0, 504 },
		{ 1972, 579, 1260, 987, 371, 999, 701, 2099, 600, 1162, 1200, 504, 0 },
	};
	public int VehicleNumber = 1; //出现问题的车辆数量，为 1，因为这是 TSP。
	public int Depot = 0;
};