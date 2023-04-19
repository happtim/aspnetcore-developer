<Query Kind="Statements" />

// 在 C# 中，可以执行以下几种类型的转换：

//隐式转换：由于这种转换始终会成功且不会导致数据丢失，因此无需使用任何特殊语法。 
//示例包括从较小整数类型到较大整数类型的转换以及从派生类到基类的转换。

//显式转换（强制转换） ：必须使用强制转换表达式，才能执行显式转换。 
//在转换中可能丢失信息时或在出于其他原因转换可能不成功时，必须进行强制转换。
//典型的示例包括从数值到精度较低或范围较小的类型的转换和从基类实例到派生类的转换。

//用户定义的转换：用户定义的转换是使用特殊方法执行，这些方法可定义为在没有基类和派生类关系的自定义类型之间启用显式转换和隐式转换。 
//有关详细信息，请参阅用户定义转换运算符。

//使用帮助程序类进行转换：若要在非兼容类型（如整数和 System.DateTime 对象，或十六进制字符串和字节数组）之间转换，
//可使用 System.BitConverter 类、System.Convert 类和内置数值类型的 Parse 方法（如 Int32.Parse）。 

//隐式转换
//对于内置数值类型，如果要存储的值无需截断或四舍五入即可适应变量，则可以进行隐式转换。
// Implicit conversion. A long can
// hold any value an int can hold, and more!
int num = 2147483647;
long bigNum = num;

//对于引用类型，隐式转换始终存在于从一个类转换为该类的任何一个直接或间接的基类或接口的情况。
//由于派生类始终包含基类的所有成员，因此不必使用任何特殊语法。

Derived d = new Derived();

// Always OK.
Base b = d;

//显式转换
//如果进行转换可能会导致信息丢失，则编译器会要求执行显式转换，显式转换也称为强制转换。

double x = 1234.7;
int a;
// Cast double to int.
a = (int)x;

//对于引用类型，如果需要从基类型转换为派生类型，则必须进行显式强制转换
d =(Derived) b;

//运行时的类型转换异常
//类型转换在运行时失败将导致引发 InvalidCastException 异常
try
{

	Derived2 d2 = (Derived2)b;

}catch(InvalidCastException ex){
	ex.Dump();
}
class Derived : Base { }
class Derived2 : Base{ }
class Base { }