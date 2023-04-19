<Query Kind="Statements" />

//值类型和引用类型是 C# 类型的两个主要类别。 值类型的变量包含类型的实例。 
//它不同于引用类型的变量，后者包含对类型实例的引用。 
//默认情况下，在分配中，通过将实参传递给方法并返回方法结果来复制变量值。 
//对于值类型变量，会复制相应的类型实例。

var p1 = new MutablePoint(1, 2);
var p2 = p1;
p2.Y = 200;
Console.WriteLine($"{nameof(p1)} after {nameof(p2)} is modified: {p1}");
Console.WriteLine($"{nameof(p2)}: {p2}");

MutateAndDisplay(p2);
Console.WriteLine($"{nameof(p2)} after passing to a method: {p2}");

void MutateAndDisplay(MutablePoint p)
{
	p.X = 100;
	Console.WriteLine($"Point mutated in a method: {p}");
}

public struct MutablePoint
{
	public int X;
	public int Y;

	public MutablePoint(int x, int y) => (X, Y) = (x, y);

	public override string ToString() => $"({X}, {Y})";
}

//如前面的示例所示，对值类型变量的操作只影响存储在变量中的值类型实例。

//值类型可以是以下种类之一：

//结构类型，用于封装数据和相关功能
//枚举类型，由一组命名常数定义，表示一个选择或选择组合


//内置值类型，也称为“简单类型”（所有简单值都是结构类型）
//整型数值类型
//浮点型数值类型
//bool，表示布尔值
//char，表示 Unicode UTF - 16 字符

