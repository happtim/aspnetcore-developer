<Query Kind="Statements" />

//最基础版本
//Expression<Func<int>> sum = () => 1 + 2;

//若要构造该表达式树，需要先构造叶节点。 叶节点是常量。 使用 Constant 方法创建节点：
var one = Expression.Constant(1, typeof(int));
var two = Expression.Constant(2, typeof(int));
//接下来，将生成加法表达式：
var addition = Expression.Add(one, two);
//生成加法表达式后，就可以创建 Lambda 表达式：
var lambda = Expression.Lambda(addition);
var compiledLambda = lambda.Compile();
compiledLambda.DynamicInvoke().Dump("无参数：分步创建");

//单语句版本
var lambda2 = Expression.Lambda<Func<int>>(
	Expression.Add(
		Expression.Constant(1, typeof(int)),
		Expression.Constant(2, typeof(int))
	)
);

lambda2.Compile().Invoke().Dump("无参数：单语句");

//带参数版本
//Expression<Func<int, int>> expression1 = m => m + 1;

//参数表达式
ParameterExpression parameterExpression = Expression.Parameter(typeof(int), "m");
//常量表达式
ConstantExpression constant = Expression.Constant(1, typeof(int));
//二元表达式
BinaryExpression addExpression = Expression.Add(parameterExpression, constant);
Expression<Func<int, int>> expression = Expression.Lambda<Func<int, int>>(addExpression, new ParameterExpression[1]
{
	parameterExpression
});
Func<int, int> func1 = expression.Compile();
func1.Invoke(5).Dump("带参数：m=> m + 1");


//多个参数版本+函数调用
//Expression<Func<double, double, double>> distanceCalc = (x, y) => Math.Sqrt(x * x + y * y);

//首先，创建 x 和 y 的参数表达式：
var xParameter = Expression.Parameter(typeof(double), "x");
var yParameter = Expression.Parameter(typeof(double), "y");

//按照你所看到的模式创建乘法和加法表达式：
var xSquared = Expression.Multiply(xParameter, xParameter);
var ySquared = Expression.Multiply(yParameter, yParameter);
var sum = Expression.Add(xSquared, ySquared);

//需要为调用 Math.Sqrt 创建方法调用表达式。
var sqrtMethod = typeof(Math).GetMethod("Sqrt", new[] { typeof(double) }) ?? throw new InvalidOperationException("Math.Sqrt not found!");
var distance = Expression.Call(sqrtMethod, sum);

//将方法调用放入 Lambda 表达式，并确保定义 Lambda 表达式的自变量：
var distanceLambda = Expression.Lambda(
	distance,
	xParameter,
	yParameter);
	
compiledLambda = distanceLambda.Compile();
compiledLambda.DynamicInvoke(1,2).Dump("带多个参数+函数调用");
