<Query Kind="Statements">
  <NuGetReference>Google.OrTools</NuGetReference>
  <Namespace>Google.OrTools.LinearSolver</Namespace>
  <RemoveNamespace>System.Data</RemoveNamespace>
</Query>

//https://developers.google.com/optimization/lp/stigler_diet

//称为斯蒂格勒饮食，以经济学诺贝尔奖得主乔治·斯蒂格勒命名，他计算出一种廉价的方式来满足基本营养需求，给定一组食物。
//他将这个问题提出为一个数学练习，而不是饮食建议，尽管计算最佳营养的概念最近已经流行起来。

#load ".\Nutrients"

//声明 LP 求解器
// Create the linear solver with the GLOP backend.
Solver solver = Solver.CreateSolver("GLOP");
if (solver is null)
{
	return;
}


//创建变量
List<Variable> foods = new List<Variable>();
for (int i = 0; i < data.Length; ++i)
{
	foods.Add(solver.MakeNumVar(0.0, double.PositiveInfinity, data[i].Name));
}
Console.WriteLine($"Number of variables = {solver.NumVariables()}");

//定义约束
List<Google.OrTools.LinearSolver.Constraint> constraints = new List<Google.OrTools.LinearSolver.Constraint>();
for (int i = 0; i < nutrients.Length; ++i)
{
	Google.OrTools.LinearSolver.Constraint constraint =
		solver.MakeConstraint(nutrients[i].Value, double.PositiveInfinity, nutrients[i].Name);
	for (int j = 0; j < data.Length; ++j)
	{
		constraint.SetCoefficient(foods[j], data[j].Nutrients[i]);
	}
	constraints.Add(constraint);
}
Console.WriteLine($"Number of constraints = {solver.NumConstraints()}");

//创建目标
Objective objective = solver.Objective();
for (int i = 0; i < data.Length; ++i)
{
	objective.SetCoefficient(foods[i], 1);
}
objective.SetMinimization();

//调用求解器并显示结果。
Solver.ResultStatus resultStatus = solver.Solve();

// Check that the problem has an optimal solution.
if (resultStatus != Solver.ResultStatus.OPTIMAL)
{
	Console.WriteLine("The problem does not have an optimal solution!");
	if (resultStatus == Solver.ResultStatus.FEASIBLE)
	{
		Console.WriteLine("A potentially suboptimal solution was found.");
	}
	else
	{
		Console.WriteLine("The solver could not solve the problem.");
		return;
	}
}

// Display the amounts (in dollars) to purchase of each food.
double[] nutrientsResult = new double[nutrients.Length];
Console.WriteLine("\nAnnual Foods:");
for (int i = 0; i < foods.Count; ++i)
{
	if (foods[i].SolutionValue() > 0.0)
	{
		Console.WriteLine($"{data[i].Name}: ${365 * foods[i].SolutionValue():N2}");
		for (int j = 0; j < nutrients.Length; ++j)
		{
			nutrientsResult[j] += data[i].Nutrients[j] * foods[i].SolutionValue();
		}
	}
}
Console.WriteLine($"\nOptimal annual price: ${365 * objective.Value():N2}");

Console.WriteLine("\nNutrients per day:");
for (int i = 0; i < nutrients.Length; ++i)
{
	Console.WriteLine($"{nutrients[i].Name}: {nutrientsResult[i]:N2} (min {nutrients[i].Value})");
}