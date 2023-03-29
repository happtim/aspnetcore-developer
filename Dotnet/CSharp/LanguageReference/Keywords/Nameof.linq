<Query Kind="Statements" />

//https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/nameof

//nameof 表达式可生成变量、类型或成员的名称作为字符串常量。 nameof 表达式在编译时进行求值，在运行时无效。

Console.WriteLine(nameof(System.Collections.Generic));  // output: Generic
Console.WriteLine(nameof(List<int>));  // output: List
Console.WriteLine(nameof(List<int>.Count));  // output: Count
Console.WriteLine(nameof(List<int>.Add));  // output: Add

Person p = new Person();
nameof(p).Dump();
nameof(Person).Dump();
nameof(p.Name).Dump();

class Person{
	public string Name {get;set;}
}

