<Query Kind="Statements" />

//DotNetty.Transport.Channels.ChannelHandlerAdapter

Target t = new Adapter();
t.targetMethod1();

public class Adapter : Target
{
	private Adaptee adaptee = new Adaptee();

	public override void targetMethod1()
	{
		this.adaptee.MethodA();
	}

	public override void targetMethod2()
	{
		this.adaptee.MethodB();
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

public abstract class Target
{
	public abstract void targetMethod1();

	public abstract void targetMethod2();
}

