<Query Kind="Statements" />

//解释表达式拼接

//使用ExpressionVisitor实现参数的替换。

Expression<Func<People, bool>> lambda1 = x => x.Age > 5;
Expression<Func<People, bool>> lambda2 = x => x.Id > 5;
Expression<Func<People, bool>> lambda3 = lambda1.And(lambda2);//且
Expression<Func<People, bool>> lambda4 = lambda1.Or(lambda2);//或
Expression<Func<People, bool>> lambda5 = lambda1.Not();//非
Do(lambda3).Dump(lambda3.ToString());
Do(lambda4).Dump(lambda4.ToString());
Do(lambda5).Dump(lambda5.ToString());

/// <summary>
/// 筛选数据执行
/// </summary>
/// <param name="func"></param>
List<People> Do(Expression<Func<People, bool>> func)
{
	List<People> people = new List<People>()
	{
		new People(){Id=4,Name="123",Age=4},
		new People(){Id=5,Name="234",Age=5},
		new People(){Id=6,Name="345",Age=6},
	};

	return people.Where(func.Compile()).ToList();
}

/// <summary>
/// 合并表达式 And Or Not扩展方法
/// </summary>
public static class ExpressionExtend
{
	/// <summary>
	/// 合并表达式 expr1 AND expr2
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="expr1"></param>
	/// <param name="expr2"></param>
	/// <returns></returns>
	public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
	{
		if (expr1 == null || expr2 == null)
		{
			throw new Exception("null不能处理");
		}
		ParameterExpression newParameter = Expression.Parameter(typeof(T), "y");
		NewExpressionVisitor visitor = new NewExpressionVisitor(newParameter);
		Expression left = visitor.Visit(expr1.Body);
		Expression right = visitor.Visit(expr2.Body);
		BinaryExpression body = Expression.And(left, right);
		return Expression.Lambda<Func<T, bool>>(body, newParameter);
	}

	/// <summary>
	/// 合并表达式 expr1 or expr2
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="expr1"></param>
	/// <param name="expr2"></param>
	/// <returns></returns>
	public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
	{
		if (expr1 == null || expr2 == null)
		{
			throw new Exception("null不能处理");
		}
		ParameterExpression newParameter = Expression.Parameter(typeof(T), "x");
		NewExpressionVisitor visitor = new NewExpressionVisitor(newParameter);
		Expression left = visitor.Visit(expr1.Body);
		Expression right = visitor.Visit(expr2.Body);
		BinaryExpression body = Expression.Or(left, right);
		return Expression.Lambda<Func<T, bool>>(body, newParameter);
	}

	/// <summary>
	/// 表达式取非
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="expr"></param>
	/// <returns></returns>
	public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
	{
		if (expr == null)
		{
			throw new Exception("null不能处理");
		}
		ParameterExpression newParameter = expr.Parameters[0];
		UnaryExpression body = Expression.Not(expr.Body);
		return Expression.Lambda<Func<T, bool>>(body, newParameter);
	}

	internal class NewExpressionVisitor : ExpressionVisitor
	{
		public ParameterExpression _NewParameter { get; private set; }
		public NewExpressionVisitor(ParameterExpression param)
		{
			this._NewParameter = param;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			return this._NewParameter;
		}
	}
}

class People
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int Age { get; set; }
}