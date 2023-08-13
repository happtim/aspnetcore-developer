<Query Kind="Statements" />

//https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/user-defined-conversion-operators

//用户定义类型可以定义与另一种类型的自定义隐式或显式转换。
//隐式转换不需要调用特殊语法，并且可以在各种情况下发生，例如，在赋值和方法调用中。

//使用operator和（implicit 或者explicit）关键字来定义隐式转化或显式转化。
//转化必需是source和target type的。在任意一个类型定义都可以。

var d = new Digit(7);

byte number = d;
Console.WriteLine(number);  // output: 7

Digit digit = (Digit)number;
Console.WriteLine(digit);  // output: 7

func(digit);
func(7);

func2(digit);
func2((Digit)number);


void func(byte input)
{
	input.Dump();
}

void func2(Digit input) 
{
	input.Dump();
}

public readonly struct Digit
{
	private readonly byte digit;

	public Digit(byte digit)
	{
		if (digit > 9)
		{
			throw new ArgumentOutOfRangeException(nameof(digit), "Digit cannot be greater than nine.");
		}
		this.digit = digit;
	}
	
	//隐式转化
	public static implicit operator byte(Digit d) => d.digit;
	//显式转化
	public static explicit operator Digit(byte b) => new Digit(b);

	public override string ToString() => $"{digit}";
}