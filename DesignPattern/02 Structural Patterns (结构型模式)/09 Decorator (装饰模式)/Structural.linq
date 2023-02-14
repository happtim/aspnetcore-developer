<Query Kind="Statements" />


//代理模式的访问控制主要在于对目标类的透明访问，也就是说我们不需要知道目标类的具体情况，
//只需要知道代理类可以帮助我们完成想要的功能就对了；而对于装饰者模式，我们是有一个目标类的对象的，

Component c = new ConcreteComponent();
Decorator dc = new ConcreteDecorator(c);
dc.Method1();


public class ConcreteComponent : Component
{
	public override void Method1()
	{
		Console.WriteLine("ConcreteComponent Method1");
	}

	public override void Method2()
	{
		Console.WriteLine("ConcreteComponent Method2");
	}
}

public class ConcreteDecorator : Decorator
{

	public ConcreteDecorator(Component component) 
		: base(component){}

	public override void Method1()
	{
		component.Method1();
		Console.WriteLine("ConcreteDecorator Method1");
	}

	public override void Method2()
	{
		component.Method2();
		Console.WriteLine("ConcreteDecorator Method2");
	}
}


public abstract class Decorator : Component
{

	protected Component component = null;

	public Decorator(Component component)
	{
		this.component = component;
	}
}

public abstract class Component
{

	public abstract void Method1();
	public abstract void Method2();
}