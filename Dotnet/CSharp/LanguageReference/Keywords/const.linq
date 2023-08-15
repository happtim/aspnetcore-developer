<Query Kind="Statements" />

//您使用const关键字来声明常量字段或局部常量。常量字段和局部常量不是变量，不能被修改。
//常量可以是数字、布尔值、字符串或空引用。不要创建常量来表示预计会随时更改的信息。
//例如，不要使用常量字段来存储服务的价格，产品版本号或公司的品牌名称。

//使用关键字const进行定义，必须在声明时进行初始化，并且不能重新赋值。
var mC = new SampleClass(11, 22);
Console.WriteLine($"x = {mC.x}, y = {mC.y}");
Console.WriteLine($"C1 = {SampleClass.C1}, C2 = {SampleClass.C2}");

class SampleClass
{
	public int x;
	public int y;
	public const int C1 = 5;
	public const int C2 = C1 + 5;

	public SampleClass(int p1, int p2)
	{
		x = p1;
		y = p2;
	}
}