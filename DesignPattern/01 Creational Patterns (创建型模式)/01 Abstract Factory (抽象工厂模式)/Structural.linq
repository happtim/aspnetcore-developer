<Query Kind="Statements" />



AbstractFactory factory1 = new ConcreteFactory1();
Client c1 = new Client(factory1);
c1.Run();


AbstractFactory factory2 = new ConcreteFactory2();
Client c2 = new Client(factory2);
c2.Run();


public class Client
{
	private AbstractProductA abstractProductA;
	private AbstractProductB abstractProductB;

	public Client(AbstractFactory abstractFactory)
	{
		abstractProductA = abstractFactory.CreateProductA();
		abstractProductB = abstractFactory.CreateProductB();
	}

	public void Run()
	{
		abstractProductB.Interact(abstractProductA);
	}

}


public class ConcreteFactory1 : AbstractFactory
{
	public override AbstractProductA CreateProductA()
	{
		return new ProductA1();
	}

	public override AbstractProductB CreateProductB()
	{
		return new ProductB1();
	}
}

public class ConcreteFactory2 : AbstractFactory
{
	public override AbstractProductA CreateProductA()
	{
		return new ProductA2();
	}

	public override AbstractProductB CreateProductB()
	{
		return new ProductB2();
	}
}
public abstract class AbstractFactory
{
	public abstract AbstractProductA CreateProductA();
	public abstract AbstractProductB CreateProductB();
}


public class ProductA1 : AbstractProductA{}

public class ProductA2 : AbstractProductA{}

public class ProductB1 : AbstractProductB
{
	public override void Interact(AbstractProductA a)
	{
		Console.WriteLine(this.GetType().Name + " interact with " + a.GetType().Name);
	}
}

public class ProductB2 : AbstractProductB
{
	public override void Interact(AbstractProductA a)
	{
		Console.WriteLine(this.GetType().Name + " interact with " + a.GetType().Name);
	}
}

public abstract class AbstractProductA{}
public abstract class AbstractProductB
{ 
	public abstract void Interact(AbstractProductA a);
}