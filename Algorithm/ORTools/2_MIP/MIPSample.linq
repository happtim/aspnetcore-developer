<Query Kind="Statements">
  <NuGetReference>Google.OrTools</NuGetReference>
  <Namespace>Google.OrTools.LinearSolver</Namespace>
</Query>

//https://developers.google.com/optimization/mip/mip_example?hl=zh-cn
//解决MIP问题

//线性优化问题需要一些变量为整数的问题被称为混合整数规划（MIPs）。
//这些变量可能以几种方式出现：
//	整数变量代表物品的数量，比如汽车或电视机，问题是决定每种物品要生产多少以最大化利润。
//	通常，这类问题可以被设置为标准线性优化 问题，附加要求是变量必须是整数。 

//	布尔变量代表取 0-1 值的决策。
//	作为一个例子，考虑一个涉及将工人分配到任务的问题（ 看分配）。要设置这种类型的问题，您可以定义布尔变量x(i,j)，如果工人i被分配到任务j，则等于 1，否则等于 0。

//尽可能增加 x +10y，但需遵循以下限制条件：
//x + 7y ≤ 17.5
//0 ≤ x ≤ 3.5
//0 ≤ y
//x，y 个整数
//由于约束条件是线性的，因此这只是一个线性优化问题，其中解决方案必须是整数。

//声明 MIP 求解器
// Create the linear solver with the SCIP backend.
Solver solver = Solver.CreateSolver("SCIP");
if (solver is null)
{
	return;
}

//定义变量
// x and y are integer non-negative variables.
Variable x = solver.MakeIntVar(0.0, double.PositiveInfinity, "x");
Variable y = solver.MakeIntVar(0.0, double.PositiveInfinity, "y");

Console.WriteLine("Number of variables = " + solver.NumVariables());

//定义限制条件
// x + 7 * y <= 17.5.
solver.Add(x + 7 * y <= 17.5);

// x <= 3.5.
solver.Add(x <= 3.5);

Console.WriteLine("Number of constraints = " + solver.NumConstraints());

//设定目标
// Maximize x + 10 * y.
solver.Maximize(x + 10 * y);

//调用求解器，显示解决方案
Solver.ResultStatus resultStatus = solver.Solve();

// Check that the problem has an optimal solution.
if (resultStatus != Solver.ResultStatus.OPTIMAL)
{
	Console.WriteLine("The problem does not have an optimal solution!");
	return;
}
Console.WriteLine("Solution:");
Console.WriteLine("Objective value = " + solver.Objective().Value());
Console.WriteLine("x = " + x.SolutionValue());
Console.WriteLine("y = " + y.SolutionValue());