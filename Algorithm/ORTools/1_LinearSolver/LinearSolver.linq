<Query Kind="Statements">
  <NuGetReference>Google.OrTools</NuGetReference>
  <Namespace>Google.OrTools.LinearSolver</Namespace>
</Query>

//线性优化

//线性优化（或线性规划）是最古老且最常用的优化领域之一，其中目标函数和约束可以编写为线性表达式。这是一个此类问题的简单示例。

//尽可能增加 3x + y，但需遵循以下限制条件：
//0 ≤ x ≤ 1
//0 ≤ y ≤ 2
//x + y ≤ 2
//此示例中的目标函数为 3x + y。目标函数和约束条件都由线性表达式指定，这使其成为线性问题。

//声明求解器。
// Create the linear solver with the GLOP backend.
Solver solver = Solver.CreateSolver("GLOP");
if (solver is null)
{
	Console.WriteLine("Could not create solver GLOP");
	return;
}

//创建变量。
// Create the variables x and y.
Variable x = solver.MakeNumVar(0.0, 1.0, "x");
Variable y = solver.MakeNumVar(0.0, 2.0, "y");

Console.WriteLine("Number of variables = " + solver.NumVariables());

//定义约束条件。 前两个约束条件 0 &leq; x ≤ 1 和 0 &leq; y ≤ 2 已由变量的定义设置。以下代码定义了约束条件 x + y &leq; 2：
// Create a linear constraint, x + y <= 2.
Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(double.NegativeInfinity, 2.0, "constraint");
constraint.SetCoefficient(x, 1);
constraint.SetCoefficient(y, 1);

Console.WriteLine("Number of constraints = " + solver.NumConstraints());

//定义目标函数。
// Create the objective function, 3 * x + y.
Objective objective = solver.Objective();
objective.SetCoefficient(x, 3);
objective.SetCoefficient(y, 1);
objective.SetMaximization();

//调用求解器并显示结果。
Console.WriteLine("Solving with " + solver.SolverVersion());
Solver.ResultStatus resultStatus = solver.Solve();
Console.WriteLine("Status: " + resultStatus);
if (resultStatus != Solver.ResultStatus.OPTIMAL)
{
	Console.WriteLine("The problem does not have an optimal solution!");
	if (resultStatus == Solver.ResultStatus.FEASIBLE)
	{
		Console.WriteLine("A potentially suboptimal solution was found");
	}
	else
	{
		Console.WriteLine("The solver could not solve the problem.");
		return;
	}
}

Console.WriteLine("Solution:");
Console.WriteLine("Objective value = " + solver.Objective().Value());
Console.WriteLine("x = " + x.SolutionValue());
Console.WriteLine("y = " + y.SolutionValue());
