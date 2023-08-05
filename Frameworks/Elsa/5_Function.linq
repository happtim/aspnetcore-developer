<Query Kind="Statements">
  <NuGetReference Prerelease="true">Elsa</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Elsa.Extensions</Namespace>
  <Namespace>Elsa.Workflows.Core.Contracts</Namespace>
  <Namespace>Elsa.Workflows.Core.Activities</Namespace>
  <Namespace>Elsa.Workflows.Core.Memory</Namespace>
  <Namespace>Elsa.Workflows.Core</Namespace>
  <Namespace>System.ComponentModel</Namespace>
  <Namespace>Elsa.Expressions.Models</Namespace>
  <Namespace>Elsa.Expressions.Helpers</Namespace>
  <Namespace>Elsa.Workflows.Core.Services</Namespace>
  <Namespace>Elsa.Workflows.Core.Models</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Setup service container.
var services = new ServiceCollection();

// Add Elsa services.
services.AddElsa();

// Build service container.
var serviceProvider = services.BuildServiceProvider();

// Resolve a workflow runner to run the workflow.
var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();
{
	// Create a workflow.
	var a = new Variable<int>();
	var b = new Variable<int>();
	var sum = new Variable<int>();

	var sumFunc = new Sum1(a,b);

	var workflow = new Sequence
	{
		Variables = { a, b ,sum },
		Activities =
		{
			new WriteLine("Enter first value"),
			new ReadLine(a),
			new WriteLine("Enter second value"),
			new ReadLine(b),
			new Sum(a, b, sum),
			new WriteLine(context => $"The sum of {a.Get(context)} and {b.Get(context)} is {sum.Get(context)}"),
			//函数直接返回参数
			sumFunc,
			//获取返回值的1种方式
			new WriteLine(context => $"The sum1 get result from activity of {a.Get(context)} and {b.Get(context)} is {sumFunc.GetResult<int>(context)}"),
			//获取返回值的2种方式
			new WriteLine(context => $"The sum1 get last result of {a.Get(context)} and {b.Get(context)} is {context.GetLastResult<int>()}"),
			//使用variable方式传参
			new Sum2(a,b),
			new WriteLine(context => $"The sum2 get last result of {a.Get(context)} and {b.Get(context)} is {context.GetLastResult<int>()}"),
			//表达式input
			new Sum3(context => a.Get<int>(context) + b.Get<int>(context)),
			new WriteLine(context => $"The sum3 get last result of {a.Get(context)} and {b.Get(context)} is {context.GetLastResult<int>()}"),
		}
	};
	
	workflowRunner.RunAsync(workflow);
}

public class Sum : CodeActivity<int>
{
	public Sum(Variable<int> a, Variable<int> b, Variable<int> result)
	{
		A = new(a);
		B = new(b);
		Result = new(result);
	}

	public Input<int> A { get; set; } = default!;
	public Input<int> B { get; set; } = default!;

	protected override void Execute(ActivityExecutionContext context)
	{
		var input1 = A.Get(context);
		var input2 = B.Get(context);
		var result = input1 + input2;
		context.SetResult(result);
	}
}

public class Sum1 : CodeActivity<int>
{
	public Sum1(Variable<int> a, Variable<int> b)
	{
		A = new(a);
		B = new(b);
	}

	public Input<int> A { get; set; } = default!;
	public Input<int> B { get; set; } = default!;

	protected override void Execute(ActivityExecutionContext context)
	{
		var input1 = A.Get(context);
		var input2 = B.Get(context);
		var result = input1 + input2;
		context.Set(Result,result);
	}
}

public class Sum2 : CodeActivity<int>
{
	public Sum2(Variable<int> a, Variable<int> b)
	{
		A = a;
		B = b;
	}

	public Variable<int> A { get; set; } = default!;
	public Variable<int> B { get; set; } = default!;

	protected override void Execute(ActivityExecutionContext context)
	{
		var input1 = A.Get(context);
		var input2 = B.Get(context);
		var result = input1 + input2;
		context.Set(Result, result);
	}
}

public class Sum3 : CodeActivity<int>
{
	public Input<int> Sum {get;set;}
	
	public Sum3(Func<ExpressionExecutionContext, int> sum)
	{
		Sum = new Input<int>(sum);
	}

	protected override void Execute(ActivityExecutionContext context)
	{
		var result = context.Get(Sum);
		context.Set(Result, result);
	}
}




