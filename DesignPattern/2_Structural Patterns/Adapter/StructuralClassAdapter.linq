<Query Kind="Statements" />



ITarget t = new Adapter();
t.targetMethod1();

public class Adapter : Adaptee, ITarget
{
	public void targetMethod1()
	{
		MethodA();
	}

	public void targetMethod2()
	{
		MethodB();
	}
}
public class Adaptee
{
	public void MethodA()
	{
		Console.Write("Adaptee MethodA");
	}

	public void MethodB() { }
	public void MethodC() { }
}

interface ITarget
{
	void targetMethod1();
	void targetMethod2();
}

