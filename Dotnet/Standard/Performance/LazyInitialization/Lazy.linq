<Query Kind="Statements" />

// 使用 Lazy<T> 延迟加载平方根函数
Lazy<Func<double, double>> lazySqrt = new Lazy<Func<double, double>>(() =>
{
	Console.WriteLine("初始化平方根函数");
	return x => Math.Sqrt(x);
});

// 使用平方根函数计算结果
double result = lazySqrt.Value(9); // 第一次访问 Value 属性会触发初始化
Console.WriteLine(result); // 输出结果：3

// 再次使用平方根函数计算结果
result = lazySqrt.Value(16); // 第二次访问 Value 属性不会触发初始化，直接返回上一次创建的函数结果
Console.WriteLine(result); // 输出结果：4