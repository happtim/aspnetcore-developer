<Query Kind="Statements" />

//将m*n个实现类转换为m+n个实现类
//https://zhuanlan.zhihu.com/p/58903776

Abstraction abst = new RefinedAbstraction(new ConcreteImplementor());
abst.method1();
((RefinedAbstraction)abst).refinedMethodA();

public class RefinedAbstraction : Abstraction
{
	public RefinedAbstraction(Implementor impl) : base(impl)
	{}

	public void refinedMethodA()
	{
		Console.WriteLine("RefinedAbstraction Method A");
	}

	public void refinedMethodB()
	{
		Console.WriteLine("RefinedAbstraction Method B");
	}

}

public abstract class Abstraction
{
	private Implementor impl;
	public Abstraction(Implementor impl)
	{
		this.impl = impl;
	}

	public void method1()
	{
		impl.implMethodX();
	}
	public void method2()
	{
		impl.implMethodY();
	}
}

class ConcreteImplementor : Implementor
{
	public void implMethodX()
	{
		Console.WriteLine("ConcreteImplementor Method X");
	}

	public void implMethodY()
	{
		Console.WriteLine("ConcreteImplementor Method Y");
	}
}

public interface Implementor
{
	void implMethodX();
	void implMethodY();
}