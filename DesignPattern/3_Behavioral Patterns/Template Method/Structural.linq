<Query Kind="Statements" />

AbstractClass absClass = new ConcreteClass();
absClass.templateMthod();

public class ConcreteClass : AbstractClass
{
	public override void method1()
	{
		Console.WriteLine("ConcreteClass Implement Method1");
	}

	public override void method2()
	{
		Console.WriteLine("ConcreteClass Implement Method2");
	}

	public override void method3()
	{
		Console.WriteLine("ConcreteClass Implement Method3");
	}
}

public abstract class AbstractClass
{
	public abstract void method1();
	public abstract void method2();
	public abstract void method3();

	public void templateMthod()
	{
		Console.WriteLine("Template Method");
		method1();
		method2();
		method3();
	}
}