<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


Person p = new Person(){Name = "Tim"};

// 加了await关键字，函数会从返回Task中获取GetAwaiter() 
//var result1 = await p.GetNameAsync1(); //返回为null，所以空指针异常
var	result2 = await p.GetNameAsync2();
var	result3 = await p.GetNameAsync3();
p = null;
var result4 = p?.GetName();
var result5 = await p?.GetNameAsync1(); //调用方为null，空指针异常 awaiter = ((person3 != null) ? person3.GetNameAsync1 () : null).GetAwaiter ();


public class Person
{
	public string Name { get; set; }

	//如何不加async 编译器不会改变函数
	public Task<string> GetNameAsync1()
	{
		return null;
	}

	//加了async 编译器会将带有async关键字修饰的方法转换为返回一个Task或Task<T>对象的异步方法。
	//它会生成一个状态机类，将方法的执行流程拆分为多个状态，
	public async Task<string> GetNameAsync2()
	{
		return null;
	}

	//不加async 但是返回Task也不会报错
	public Task<string> GetNameAsync3()
	{
		return Task.FromResult<string>(null);
	}

	public string GetName()
	{
		return Name;
	}
}

