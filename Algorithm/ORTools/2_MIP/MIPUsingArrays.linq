<Query Kind="Statements">
  <NuGetReference>Google.OrTools</NuGetReference>
  <Namespace>Google.OrTools.LinearSolver</Namespace>
</Query>

//前一节展示了如何解决一个只有少量变量和约束的 MIP 问题，这些变量和约束是单独定义的。
//对于更大的问题，更方便的方法是通过循环遍历数组来定义变量和约束。下一个示例将说明这一点。

//新的问题
//Maximize 7x1 + 8x2 + 2x3 + 9x4 + 6x5 subject to the following constraints:
//
//5 x1 + 7 x2 + 9 x3 + 2 x4 + 1 x5 ≤ 250
//18 x1 + 4 x2 - 9 x3 + 10 x4 + 12 x5 ≤ 285
//4 x1 + 7 x2 + 3 x3 + 8 x4 + 5 x5 ≤ 211
//5 x1 + 13 x2 + 16 x3 + 3 x4 - 7 x5 ≤ 315
//where x1, x2, ..., x5 are non - negative integers.

//声明 MIP 求解器
// Create the linear solver with the SCIP backend.
Solver solver = Solver.CreateSolver("SCIP");
if (solver is null)
{
	return;
}

//创建数据
DataModel data = new DataModel();

//定义变量
Variable[] x = new Variable[data.NumVars];
for (int j = 0; j < data.NumVars; j++)
{
	x[j] = solver.MakeIntVar(0.0, double.PositiveInfinity, $"x_{j}");
}
Console.WriteLine("Number of variables = " + solver.NumVariables());

//定义约束
for (int i = 0; i < data.NumConstraints; ++i)
{
	Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(0, data.Bounds[i], "");
	for (int j = 0; j < data.NumVars; ++j)
	{
		constraint.SetCoefficient(x[j], data.ConstraintCoeffs[i, j]);
	}
}
Console.WriteLine("Number of constraints = " + solver.NumConstraints());

//定义目标
Objective objective = solver.Objective();
for (int j = 0; j < data.NumVars; ++j)
{
	objective.SetCoefficient(x[j], data.ObjCoeffs[j]);
}
objective.SetMaximization();

//调用求解器，显示解决方案
Solver.ResultStatus resultStatus = solver.Solve();

// Check that the problem has an optimal solution.
if (resultStatus != Solver.ResultStatus.OPTIMAL)
{
	Console.WriteLine("The problem does not have an optimal solution!");
	return;
}

Console.WriteLine("Solution:");
Console.WriteLine("Optimal objective value = " + solver.Objective().Value());

for (int j = 0; j < data.NumVars; ++j)
{
	Console.WriteLine("x[" + j + "] = " + x[j].SolutionValue());
}

class DataModel
{
	public double[,] ConstraintCoeffs = {
		{ 5, 7, 9, 2, 1 },
		{ 18, 4, -9, 10, 12 },
		{ 4, 7, 3, 8, 5 },
		{ 5, 13, 16, 3, -7 },
	};
	public double[] Bounds = { 250, 285, 211, 315 };
	public double[] ObjCoeffs = { 7, 8, 2, 9, 6 };
	public int NumVars = 5;
	public int NumConstraints = 4;
}

