<Query Kind="Statements">
  <NuGetReference Version="6.0.3">UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities</Namespace>
  <Namespace>System.Activities.Statements</Namespace>
</Query>

#load ".\Activities"

// Create a workflow.
var a = new Variable<string>();
var b = new Variable<string>();
var sum = new Variable<int>();

var workflow = new Sequence
{
	Variables = { a, b, sum },
	Activities =
	{
		new WriteLine{ Text = "Enter first value" },
		new ReadLine
		{
			Result = new OutArgument<string>(a)
		},
		new WriteLine { Text = "Enter second value" },
		new ReadLine
		{
			Result = new OutArgument<string>(b)
		},
		new Sum
		{
			Number1 = new InArgument<string>(a),
			Number2 = new InArgument<string>(b),
			Result = new OutArgument<int>(sum),
		},
		new WriteLine { Text = new InArgument<string>( context => $"The sum of {a.Get(context)} and {b.Get(context)} is {sum.Get(context)}")},

	}
};
WorkflowInvoker.Invoke(workflow);

public sealed class Sum : NativeActivity<int>
{
	// 定义两个int类型的输入参数
    public InArgument<string> Number1 { get; set; }
    public InArgument<string> Number2 { get; set; }
	
	public Sum()
	{}
	
	public Sum(Variable<string> a, Variable<string> b , Variable<int> sum)
	{
		Number1 = new InArgument<string>(a);
		Number2 = new InArgument<string>(b);
		Result =  new OutArgument<int>(sum);
	}

    protected override void Execute(NativeActivityContext context)
    {
        // 从输入参数中获取两个整数的值
        int number1 = Convert.ToInt32( Number1.Get(context));
        int number2 = Convert.ToInt32( Number2.Get(context));

        // 计算两个整数的和，并将结果设置为活动的输出
        int sum = number1 + number2;
        context.SetValue(Result, sum);
    }
}