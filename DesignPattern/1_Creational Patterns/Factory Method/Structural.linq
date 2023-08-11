<Query Kind="Statements" />



Creator creator = new ConcreteCreator();
Product product = creator.factoryMethod();
product.method1();

public class ConcreteProduct : Product
{
	public override void method1()
	{
		Console.WriteLine("ConcreteProduct method1");
	}

	public override void method2()
	{
		Console.WriteLine("ConcreteProduct method2");
	}

	public override void method3()
	{
		Console.WriteLine("ConcreteProduct method3");
	}
}

public class ConcreteCreator : Creator
{
	public override Product factoryMethod()
	{
		return new ConcreteProduct();
	}
}

public abstract class Creator
{
	public abstract Product factoryMethod();
}

public abstract class Product
{
	public abstract void method1();
	public abstract void method2();
	public abstract void method3();
}