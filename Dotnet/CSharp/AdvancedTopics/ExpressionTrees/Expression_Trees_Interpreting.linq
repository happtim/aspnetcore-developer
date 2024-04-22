<Query Kind="Statements" />

//解释表达式

Expression<Func<int, int>> expression = (num) => num + 5;
Console.WriteLine($"NodeType:{expression.NodeType}");
Console.WriteLine($"Body:{expression.Body}");
Console.WriteLine($"Body Type: {expression.Body.GetType()}"); //BinaryExpression 二元表达式，也是一个二叉树
Console.WriteLine($"Body NodeType: {expression.Body.NodeType}");


if (expression is LambdaExpression lambda)
{
	//分解表达式
	ParameterExpression param = lambda.Parameters[0];
	BinaryExpression operation = (BinaryExpression)lambda.Body;
	ParameterExpression left = (ParameterExpression)operation.Left;
	ConstantExpression right = (ConstantExpression)operation.Right;
	Console.WriteLine("Decomposed expression: {0} => {1} {2} {3}",
					  param.Name, left.Name, operation.NodeType, right.Value);
}
expression.Dump();

//一元表达式
ConstantExpression _consNum = Expression.Constant(5, typeof(int));
UnaryExpression _unaryPlus = Expression.Decrement(_consNum);  // 一元表达式。
Expression<Func<int>> _unaryLam = Expression.Lambda<Func<int>>(_unaryPlus);
_unaryLam.Compile()().Dump("一元表达式");