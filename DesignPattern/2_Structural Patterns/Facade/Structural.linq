<Query Kind="Statements" />

Facade facade = new Facade();
facade.MethodA();
facade.MethodB();

public class Facade
{
	private SubSystemOne one;
	private SubSystemTwo two;
	private SubSystemThree three;
	private SubSystemFour four;


	public Facade()
	{
		one = new SubSystemOne();
		two = new SubSystemTwo();
		three = new SubSystemThree();
		four = new SubSystemFour();
	}

	public void MethodA()
	{
		Console.WriteLine("Facede MethodA");
		one.MethodOne();
		two.MethodTwo();
		four.MethodFour();
	}

	public void MethodB()
	{
		Console.WriteLine("Facede MethodB");
		two.MethodTwo();
		three.MethodThree();
	}

}

public class SubSystemOne
{
	public void MethodOne()
	{
		Console.WriteLine("SubSystemOne Method");
	}
}

public class SubSystemTwo
{
	public void MethodTwo()
	{
		Console.WriteLine("SubSystemTwo Method");
	}
}

public class SubSystemThree
{
	public void MethodThree()
	{
		Console.WriteLine("SubSystemThree Method");
	}
}

public class SubSystemFour
{
	public void MethodFour()
	{
		Console.WriteLine("SubSystemFour Method");
	}
}