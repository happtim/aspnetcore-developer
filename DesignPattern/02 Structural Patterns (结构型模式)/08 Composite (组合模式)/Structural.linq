<Query Kind="Statements" />

////DotNetty.Buffers.CompositeByteBuffer

Component c1 = new Left();
Component c2 = new Left();

Component composite = new Composite();
composite.add(c1);
composite.add(c2);

composite.method1();

public class Composite : Component
{

	private List<Component> list = new List<Component>();

	public override void add(Component c)
	{
		list.Add(c);
	}

	public override Component getChild(int i)
	{
		return list[i];
	}

	public override void method1()
	{
		foreach (var c in list)
		{
			c.method1();
		}
	}

	public override void method2()
	{
		foreach (var c in list)
		{
			c.method2();
		}
	}

	public override void remove(Component c)
	{
		list.Remove(c);
	}
}

public class Left : Component
{
	public override void add(Component c)
	{
		throw new NotImplementedException();
	}

	public override Component getChild(int i)
	{
		throw new NotImplementedException();
	}

	public override void method1()
	{
		Console.WriteLine("Left method1");
	}

	public override void method2()
	{
		Console.WriteLine("Left method2");
	}

	public override void remove(Component c)
	{
		throw new NotImplementedException();
	}
}

public abstract class Component
{
	public abstract void method1();
	public abstract void method2();
	public abstract void add(Component c);
	public abstract void remove(Component c);
	public abstract Component getChild(int i);
}