<Query Kind="Statements" />

//装箱是将值类型转换为 object 类型或由此值类型实现的任何接口类型的过程。 常见语言运行时 (CLR) 对值类型进行装箱时，会将值包装在 System.Object 实例中并将其存储在托管堆中。
// 取消装箱将从对象中提取值类型。 装箱是隐式的；取消装箱是显式的。 装箱和取消装箱的概念是类型系统 C# 统一视图的基础，其中任一类型的值都被视为一个对象。

//下例将整型变量 i 进行了装箱并分配给对象 o。

int i = 123;
// The following line boxes i.
object o = i;

//然后，可以将对象 o 取消装箱并分配给整型变量 i：

o = 123;
i = (int)o;  // unboxing