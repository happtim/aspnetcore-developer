<Query Kind="Statements" />

//Expression 和 委托的区别？
//Expression是只读的，不可执行，它只能被解析和分析，无法直接执行。
//而委托是可执行的，可以直接调用相关的方法

//下面代码演示了达式转换为委托
Expression<Func<People, bool>> expression = p => p.Id == 10;
Func<People, bool> func = expression.Compile(); // Create Delegate
func.Invoke(new People() // Invoke Delegate
{
	Id = 10,
	Name = "张三"
}).Dump();


//下面的代码示例演示如何通过创建 lambda 表达式并执行它来执行代表幂运算的表达式树。
// The expression tree to execute.
BinaryExpression be = Expression.Power(Expression.Constant(2d), Expression.Constant(3d));

// Create a lambda expression.
Expression<Func<double>> le = Expression.Lambda<Func<double>>(be);

// Compile the lambda expression.
Func<double> compiledExpression = le.Compile();

// Execute the lambda expression.
compiledExpression().Dump("2^3");




class People
{
	public int Id { get; set; }
	public string Name { get; set; }
}