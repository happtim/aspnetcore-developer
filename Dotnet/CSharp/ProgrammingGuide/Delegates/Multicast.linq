<Query Kind="Statements" />


MyDelegate myDelegate = Method1; // 实例化委托并指定第一个方法
myDelegate += Method2; // 将第二个方法添加到委托中
myDelegate += Method3; // 将第三个方法添加到委托中

myDelegate(); // 调用委托，会按照添加的顺序依次调用方法

myDelegate -= Method3;
Console.WriteLine("\nRemove Method3");

myDelegate(); // 调用委托，会按照添加的顺序依次调用方法

static void Method1()
{
	Console.WriteLine("Method1");
}

static void Method2()
{
	Console.WriteLine("Method2");
}

static void Method3()
{
	Console.WriteLine("Method3");
}

delegate void MyDelegate(); // 定义一个委托类型