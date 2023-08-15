<Query Kind="Statements" />


//在字段声明中，“readonly”表示只有在声明的一部分或者在同一类的构造函数中才能对字段进行赋值。
//一个只读字段可以在字段声明和构造函数中被赋值和重新赋值多次。

var age = new Age(11);

class Age
{
	private readonly int _year = 1;
	public Age(int year)
	{
		_year = year;
	}
	void ChangeYear()
	{
		//_year = 1967; // Compile error if uncommented.
	}
}