<Query Kind="Statements">
  <NuGetReference>DynamicExpresso.Core</NuGetReference>
  <Namespace>DynamicExpresso</Namespace>
</Query>


//返回值
var target = new Interpreter();
double result = target.Eval<double>("Math.Pow(x, y) + 5",
					new Parameter("x", typeof(double), 10),
					new Parameter("y", typeof(double), 2));

result.Dump("Return value");

//设置变量
target = new Interpreter().SetVariable("myVar", 23);
target.Eval("myVar").Dump("Variables");

//设置自定义函数
Func<double, double, double> pow = (x, y) => Math.Pow(x, y);
target = new Interpreter().SetFunction("pow", pow);
target.Eval("pow(3,2)").Dump("Set Function");

//参数
target = new Interpreter();

var parameters = new[] {
	new Parameter("x", 23),
	new Parameter("y", 7)
};

target.Eval("x + y", parameters).Dump("Parameters");

//表达式解析一次，多次使用
target = new Interpreter();

parameters = new[] {
	new Parameter("x", typeof(int)),
	new Parameter("y", typeof(int))
};

var myFunc = target.Parse("x + y", parameters);

myFunc.Invoke(23,7).Dump("invoke it multiple times");
myFunc.Invoke(32,-2).Dump("invoke it multiple times");

//this 关键字
target = new Interpreter();

// 'this' is treated as a special identifier and can be accessed implicitly 
target.SetVariable("this", new Customer { Name = "John" });
target.Eval("this.Name").Dump("this keyword");

//Math
target.Eval("Math.Sqrt(25)").Dump("Math.Sqrt");
target.Eval("Math.Ceiling(4.2)").Dump("Math.Ceiling");
target.Eval("Math.Max(5,10)").Dump("Math.Max");

//Convert
target.Eval("Convert.ToInt32(\"123\")").Dump("Convert.ToInt32");

var customers = new List<Customer> {
		new Customer() { Name = "David", Age = 31, Gender = 'M' },
		new Customer() { Name = "Mary", Age = 29, Gender = 'F' },
		new Customer() { Name = "Jack", Age = 2, Gender = 'M' },
		new Customer() { Name = "Marta", Age = 1, Gender = 'F' },
		new Customer() { Name = "Moses", Age = 120, Gender = 'M' },
	};
	
string whereExpression = "customer.Age > 18 && customer.Gender == 'F'";

Func<Customer, bool> dynamicWhere = target.ParseAsDelegate<Func<Customer, bool>>(whereExpression, "customer");

customers.Where(dynamicWhere ).Count().Dump("ParseAsDelegate");

Expression<Func<Customer, bool>> expression = target.ParseAsExpression<Func<Customer, bool>>(whereExpression, "customer");

customers.Where(expression.Compile()).Count().Dump("ParseAsExpression");

//可以调用任何标准的 .NET 方法、字段、属性或构造函数。

var customer = new Customer{ Name = "John", Age = 21, Gender = 'M'};

target =  new Interpreter()
  .SetVariable("x", customer);
  
target.Eval("x.Name").Dump("x.Name");
target.Eval("x.ToString()").Dump();

var xx = new int[] { 10, 30, 4 };
target = new Interpreter()
	.Reference(typeof(System.Linq.Enumerable))
	.SetVariable("xx", xx);
target.Eval("xx.Count()").Dump("Enumerable.Count()");

// 对 lambda 表达式有部分支持
var x = new string[] { "this", "is", "awesome" };
var options = InterpreterOptions.Default | InterpreterOptions.LambdaExpressions; // enable lambda expressions
target = new Interpreter(options)
	.SetVariable("x", x);
target.Eval<IEnumerable<string>>("x.Where(str => str.Length > 5).Select(str => str.ToUpper())").Dump("Lambda expressions");

//标识符（变量、类型、参数）检测
//有时你需要在解析表达式之前检查哪些标识符（变量、类型、参数）被使用。可能是因为你想验证它，
var detectedIdentifiers = target.DetectIdentifiers("x + y");
detectedIdentifiers.Dump("检测");

class Customer
{
	public string Name { get; set; }
	public int Age { get; set; }
	public char Gender { get; set; }

	public override string ToString()
	{
		return $"{Name} - {Age} ({Gender})";
	}
}