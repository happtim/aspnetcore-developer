<Query Kind="Statements" />

//ExpressionVisitor

//Expression是通过访问者模式进行解析的，官方提供了ExpressionVisitor抽象类
//ExpressionVisitor的Visit方法是解析表达式目录树的一个入口，Visit方法判断Expression是一个什么表达式目录树，走不同的细分方法进行进一步解析
//ExpressionVisitor的VisitBinary方法是对二元表达式的解析，所有复杂的表达式都会拆解成二元表达式进行解析

Expression<Func<int, int, int>> exp = (m, n) => m * n + 2;
Console.WriteLine(exp.ToString());
OperationsVisitor visitor = new OperationsVisitor();
Expression expNew = visitor.Visit(exp);
Console.WriteLine(expNew.ToString());

/// <summary>
/// 自定义Visitor
/// </summary>
public class OperationsVisitor : ExpressionVisitor
{
	/// <summary>
	/// 覆写父类方法；//二元表达式的访问
	/// 把表达式目录树中相加改成相减，相乘改成相除
	/// </summary>
	/// <param name="b"></param>
	/// <returns></returns>
	protected override Expression VisitBinary(BinaryExpression b)
	{
		if (b.NodeType == ExpressionType.Add)//相加
		{
			Expression left = this.Visit(b.Left);
			Expression right = this.Visit(b.Right);
			return Expression.Subtract(left, right);//相减
		}
		else if (b.NodeType == ExpressionType.Multiply) //相乘
		{
			Expression left = this.Visit(b.Left);
			Expression right = this.Visit(b.Right);
			return Expression.Divide(left, right); //相除
		}
		return base.VisitBinary(b);
	}
}