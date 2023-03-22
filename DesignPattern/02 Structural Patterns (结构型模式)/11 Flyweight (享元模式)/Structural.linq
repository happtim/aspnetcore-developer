<Query Kind="Statements" />


FlyweightFactory factory = new FlyweightFactory();
Flyweight a = factory.getFlyweight("aaa");
Flyweight a2 = factory.getFlyweight("aaa");

(a == a2) .Dump();

public class FlyweightFactory
{
	private Dictionary<string, Flyweight> pool = new Dictionary<string, Flyweight>();

	public Flyweight getFlyweight(string name)
	{
		if (pool.ContainsKey(name))
		{
			return pool[name];
		}
		else
		{
			Flyweight fly = new Flyweight();
			pool.Add(name, fly);
			return fly;
		}

	}

}

public class Flyweight
{
	public void methodA()
	{
		Console.WriteLine("Flyweight methodA");
	}

	public void methodB()
	{
		Console.WriteLine("Flyweight methodB");
	}
}