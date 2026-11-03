<Query Kind="Statements" />

//范型中空值的处理方式

new Test().Id.Dump();
new Test().Id2.Dump();

new Test2<Guid>().Id.Dump();
new Test2<Guid>().Id2.Dump();

new Test2<Guid?>().Id.Dump();
new Test2<Guid?>().Id2.Dump();

new Test3<Guid>().Id.Dump();
new Test3<Guid>().Id2.Dump();

public class Test
{
	public Guid Id {get;set;}
	public Guid? Id2 {get;set;}
}

public class Test2<TKey>
{
	public TKey Id {get;set;}
	public TKey? Id2 {get;set;}
}

public class Test3<TKey>
	where TKey:struct
{
	public TKey Id { get; set; }
	public TKey? Id2 { get; set; } = null;
}