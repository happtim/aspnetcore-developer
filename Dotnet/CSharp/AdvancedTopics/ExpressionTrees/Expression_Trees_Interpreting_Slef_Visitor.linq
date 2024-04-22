<Query Kind="Statements" />

//解释表达式
//在前面的代码示例中，你会注意到很多重复。
//并生成一个更加通用的表达式节点访问者。

Expression<Func<int, int, int>> addition = (a, b) => a + b;

Visitor.CreateFromExpression(addition).Visit("^");


//二元表达式默认是左结合的。
Expression<Func<int, int>> sum = (a) => 1 + a + 3 + 4;

//右结合
Expression<Func<int>> sum1 = () => 1 + (2 + (3 + 4));
//左结合
Expression<Func<int>> sum2 = () => ((1 + 2) + 3) + 4;

Visitor.CreateFromExpression(sum).Visit("*");


// Base Visitor class:
public abstract class Visitor
{
	private readonly Expression node;

	protected Visitor(Expression node) => this.node = node;

	public abstract void Visit(string prefix);

	public ExpressionType NodeType => node.NodeType;
	public static Visitor CreateFromExpression(Expression node) =>
		node.NodeType switch
		{
			ExpressionType.Constant => new ConstantVisitor((ConstantExpression)node),
			ExpressionType.Lambda => new LambdaVisitor((LambdaExpression)node),
			ExpressionType.Parameter => new ParameterVisitor((ParameterExpression)node),
			ExpressionType.Add => new BinaryVisitor((BinaryExpression)node),
			_ => throw new NotImplementedException($"Node not processed yet: {node.NodeType}"),
		};
}

// Lambda Visitor
public class LambdaVisitor : Visitor
{
	private readonly LambdaExpression node;
	public LambdaVisitor(LambdaExpression node) : base(node) => this.node = node;

	public override void Visit(string prefix)
	{
		Console.WriteLine($"{prefix}This expression is a {NodeType} expression type");
		Console.WriteLine($"{prefix}The name of the lambda is {((node.Name == null) ? "<null>" : node.Name)}");
		Console.WriteLine($"{prefix}The return type is {node.ReturnType}");
		Console.WriteLine($"{prefix}The expression has {node.Parameters.Count} argument(s). They are:");
		// Visit each parameter:
		foreach (var argumentExpression in node.Parameters)
		{
			var argumentVisitor = CreateFromExpression(argumentExpression);
			argumentVisitor.Visit(prefix + "----");
		}
		Console.WriteLine($"{prefix}The expression body is:");
		// Visit the body:
		var bodyVisitor = CreateFromExpression(node.Body);
		bodyVisitor.Visit(prefix + "----");
	}
}

// Binary Expression Visitor:
public class BinaryVisitor : Visitor
{
	private readonly BinaryExpression node;
	public BinaryVisitor(BinaryExpression node) : base(node) => this.node = node;

	public override void Visit(string prefix)
	{
		Console.WriteLine($"{prefix}This binary expression is a {NodeType} expression");
		var left = CreateFromExpression(node.Left);
		Console.WriteLine($"{prefix}The Left argument is:");
		left.Visit(prefix + "----");
		var right = CreateFromExpression(node.Right);
		Console.WriteLine($"{prefix}The Right argument is:");
		right.Visit(prefix + "----");
	}
}

// Parameter visitor:
public class ParameterVisitor : Visitor
{
	private readonly ParameterExpression node;
	public ParameterVisitor(ParameterExpression node) : base(node)
	{
		this.node = node;
	}

	public override void Visit(string prefix)
	{
		Console.WriteLine($"{prefix}This is an {NodeType} expression type");
		Console.WriteLine($"{prefix}Type: {node.Type}, Name: {node.Name}, ByRef: {node.IsByRef}");
	}
}

// Constant visitor:
public class ConstantVisitor : Visitor
{
	private readonly ConstantExpression node;
	public ConstantVisitor(ConstantExpression node) : base(node) => this.node = node;

	public override void Visit(string prefix)
	{
		Console.WriteLine($"{prefix}This is an {NodeType} expression type");
		Console.WriteLine($"{prefix}The type of the constant value is {node.Type}");
		Console.WriteLine($"{prefix}The value of the constant value is {node.Value}");
	}
}