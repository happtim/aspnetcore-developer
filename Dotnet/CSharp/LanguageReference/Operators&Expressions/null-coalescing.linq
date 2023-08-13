<Query Kind="Statements" />

//??
//如果左操作数的值不为 null，则 null 合并运算符 ?? 返回该值；否则，它会计算右操作数并返回其结果。

//??=
//仅当左操作数的计算结果为 null 时，Null 合并赋值运算符 ??= 才会将其右操作数的值赋值给其左操作数。
//如果左操作数的计算结果为非 null，则 ??= 运算符不会计算其右操作数。


int? a = null;
int b = a ?? -1;
Console.WriteLine(b);  // output: -1


Person p = new Person();
//p.Name = null;


//??= 两者相同使用
//if (a is null)
//{
//	a = -1;
//}

a ??= -1;
Console.WriteLine(a);  // output: -1


public class Person
{
	private string name ;
	public string Name
	{
		get => name;
		set => name = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null");
	}
}


