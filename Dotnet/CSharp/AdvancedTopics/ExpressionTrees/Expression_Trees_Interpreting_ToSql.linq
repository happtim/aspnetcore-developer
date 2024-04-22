<Query Kind="Statements" />

//实现ORM框架Expression映射成SQL

//普通多条件
Expression<Func<People, bool>> lambda = x => x.Age > 5
											 && x.Id > 5
											 && x.Name.StartsWith("1") //  like '1%'
											 && x.Name.EndsWith("1") //  like '%1'
											 && x.Name.Contains("1");//  like '%1%'
ConditionBuilderVisitor vistor = new ConditionBuilderVisitor();
vistor.Visit(lambda);
ConstantSqlString<People>.GetQuerySql(vistor.Condition()).Dump("普通多条件");


//外部参数变量
string name = "AAA";
vistor.Visit(lambda = x => x.Age > 5 && x.Name == name || x.Id > 5);
ConstantSqlString<People>.GetQuerySql(vistor.Condition()).Dump("外部参数变量");

//内部常量多条件
vistor.Visit(lambda = x => x.Age > 5 || (x.Name == "A" && x.Id > 5));
ConstantSqlString<People>.GetQuerySql(vistor.Condition()).Dump("内部常量多条件");

public class ConditionBuilderVisitor : ExpressionVisitor
{
	private Stack<string> _StringStack = new Stack<string>();

	/// <summary>
	/// 返回拼装好的sql条件表达式
	/// </summary>
	/// <returns></returns>
	public string Condition()
	{
		string condition = string.Concat(this._StringStack.ToArray());
		this._StringStack.Clear();
		return condition;
	}

	/// <summary>
	/// 如果是二元表达式
	/// </summary>
	/// <param name="node"></param>
	/// <returns></returns>
	protected override Expression VisitBinary(BinaryExpression node)
	{
		if (node == null) throw new ArgumentNullException("BinaryExpression");

		this._StringStack.Push(")");
		base.Visit(node.Right);//解析右边
		this._StringStack.Push(" " + ToSqlOperator(node.NodeType) + " ");
		base.Visit(node.Left);//解析左边
		this._StringStack.Push("(");

		return node;
	}

	/// <summary>
	/// 解析属性
	/// </summary>
	/// <param name="node"></param>
	/// <returns></returns>
	protected override Expression VisitMember(MemberExpression node)
	{
		if (node == null) throw new ArgumentNullException("MemberExpression");
		if (node.Expression is ConstantExpression)
		{
			var value1 = this.InvokeValue(node);
			var value2 = this.ReflectionValue(node);
			this._StringStack.Push("'" + value2 + "'");
		}
		else
		{
			this._StringStack.Push(" [" + node.Member.Name + "] ");
		}
		return node;
	}

	private string ToSqlOperator(ExpressionType type)
	{
		switch (type)
		{
			case (ExpressionType.AndAlso):
			case (ExpressionType.And):
				return "AND";
			case (ExpressionType.OrElse):
			case (ExpressionType.Or):
				return "OR";
			case (ExpressionType.Not):
				return "NOT";
			case (ExpressionType.NotEqual):
				return "<>";
			case ExpressionType.GreaterThan:
				return ">";
			case ExpressionType.GreaterThanOrEqual:
				return ">=";
			case ExpressionType.LessThan:
				return "<";
			case ExpressionType.LessThanOrEqual:
				return "<=";
			case (ExpressionType.Equal):
				return "=";
			default:
				throw new Exception("不支持该方法");
		}
	}

	private object InvokeValue(MemberExpression member)
	{
		var objExp = Expression.Convert(member, typeof(object));//struct需要
		return Expression.Lambda<Func<object>>(objExp).Compile().Invoke();
	}

	private object ReflectionValue(MemberExpression member)
	{
		var obj = (member.Expression as ConstantExpression).Value;
		return (member.Member as FieldInfo).GetValue(obj);
	}

	/// <summary>
	/// 常量表达式
	/// </summary>
	/// <param name="node"></param>
	/// <returns></returns>
	protected override Expression VisitConstant(ConstantExpression node)
	{
		if (node == null) throw new ArgumentNullException("ConstantExpression");
		this._StringStack.Push("" + node.Value + "");
		return node;
	}
	/// <summary>
	/// 方法表达式
	/// </summary>
	/// <param name="m"></param>
	/// <returns></returns>
	protected override Expression VisitMethodCall(MethodCallExpression m)
	{
		if (m == null) throw new ArgumentNullException("MethodCallExpression");

		string format;
		switch (m.Method.Name)
		{
			case "StartsWith":
				format = "({0} LIKE '{1}%')";
				break;
			case "Contains":
				format = "({0} LIKE '%{1}%')";
				break;
			case "EndsWith":
				format = "({0} LIKE '%{1}')";
				break;
			default:
				throw new NotSupportedException(m.NodeType + " is not supported!");
		}
		this.Visit(m.Object);
		this.Visit(m.Arguments[0]);
		string right = this._StringStack.Pop();
		string left = this._StringStack.Pop();
		this._StringStack.Push(String.Format(format, left, right));
		return m;
	}
}

public class ConstantSqlString<T>
{
	/// <summary>
	/// 泛型缓存，一个类型一个缓存
	/// </summary>
	private static string FindSql = null;

	/// <summary>
	/// 获取查询sql
	/// </summary>
	static ConstantSqlString()
	{
		Type type = typeof(T);
		FindSql = $"Select {string.Join(',', type.GetProperties().Select(c => $"[{c.Name}]").ToList())} from {type.Name}";
	}

	/// <summary>
	/// 获取查询sql+条件筛选
	/// </summary>
	/// <param name="exp"></param>
	/// <returns></returns>
	public static string GetQuerySql(string exp)
	{
		return $"{FindSql} where {exp}";
	}
}

class People
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int Age { get; set; }
}