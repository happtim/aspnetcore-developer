<Query Kind="Statements" />


//代理模式的访问控制主要在于对目标类的透明访问，也就是说我们不需要知道目标类的具体情况，
//只需要知道代理类可以帮助我们完成想要的功能就对了。
//而对于装饰者模式，我们是有一个目标类的对象的，

//装饰者与被装饰者有相同的接口。
//装饰者给被装饰者动态修改行为。

//装饰模式又名包装(Wrapper)模式。装饰模式以对客户端透明的方式扩展对象的功能，是继承关系的一个替代方案。
//换言之，客户端并不会觉得对象在装饰前和装饰后有什么不同。装饰模式可以在不使用创造更多子类的情况下，将对象的功能加以扩展。
//有个多个子类，需要增加要给行为，所以不能给所有子类都继承一个类。

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