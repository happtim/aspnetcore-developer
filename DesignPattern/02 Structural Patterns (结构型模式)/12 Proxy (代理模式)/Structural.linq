<Query Kind="Statements" />


Subject s = new Proxy();
s.SomeMethod();
s.SomeMethod2();
s.Request();


public class Proxy : Subject
{

	private RealSubject real;

	public override void Request()
	{
		if (real == null)
		{
			real = new RealSubject();
		}

		real.Request();
	}

	public override void SomeMethod()
	{
		Console.WriteLine("SomeMethod");
	}

	public override void SomeMethod2()
	{
		Console.WriteLine("SomeMethod2");
	}
}

public abstract class Subject
{
	public abstract void Request();
	public abstract void SomeMethod();
	public abstract void SomeMethod2();
}

public class RealSubject : Subject
{

	public RealSubject()
	{
		Console.WriteLine("Init Realsubject ...");
		Thread.Sleep(1000);
	}

	public override void Request()
	{
		Console.WriteLine("RealSubject Request1");
	}

	public override void SomeMethod()
	{
		Console.WriteLine("RealSubject Request2");
	}

	public override void SomeMethod2()
	{
		Console.WriteLine("RealSubject Request2");
	}
}