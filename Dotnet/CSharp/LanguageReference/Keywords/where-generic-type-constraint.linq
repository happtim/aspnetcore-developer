<Query Kind="Expression" />

//泛型定义中的 where 子句指定对用作泛型类型、方法、委托或本地函数中类型参数的参数类型的约束。 
//约束可指定接口、基类或要求泛型类型为引用、值或非托管类型。 
//约束声明类型参数必须具有的功能，并且约束必须位于任何声明的基类或实现的接口之后。

//where T: class
//表示T必须是引用类型，即类、接口、委托或数组类型。
//
//where T: struct
//表示T必须是值类型，即除了类、接口、委托或数组类型以外的结构体类型。
//
//where T: new()
//表示T必须具有无参数构造函数。
//
//where T: SomeBaseClass
//表示T必须是指定的基类SomeBaseClass或其子类。
//
//where T: ISomeInterface
//表示T必须实现指定的接口ISomeInterface。
//
//where T: U
//表示T必须是U或其子类。其中，U可以是类、接口、结构体或泛型类型参数。
//可以使用where子句来指定泛型类型参数的约束。例如，以下代码定义了一个泛型方法，其中T必须是实现了IComparable接口的引用类型：