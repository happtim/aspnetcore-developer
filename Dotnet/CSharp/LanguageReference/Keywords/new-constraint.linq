<Query Kind="Statements" />

//new 约束指定泛型类或方法声明中的类型实参必须有公共的无参数构造函数。 若要使用 new 约束，则该类型不能为抽象类型。


new ItemFactory<Item>().GetNewItem().Dump();


class Item
{
	
}

class ItemFactory<T> where T : new()
{
	public T GetNewItem()
	{
		return new T();
	}
}


//当与其他约束一起使用时，new() 约束必须最后指定：
public class ItemFactory2<T>
	where T : IComparable, new()
{ }