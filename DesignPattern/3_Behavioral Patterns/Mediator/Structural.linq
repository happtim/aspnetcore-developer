<Query Kind="Statements" />

//Observer 发布订阅直接产生关系
//Mediator 发布订阅中间加了一个中介者，由中介者管理不同对象的订阅发布关系。

Mediator mediator = new ConcreteMediator();

ConcreteColleague1 c1 = new ConcreteColleague1();
c1.SetMediator(mediator);
mediator.RegisterColleagues(c1);

ConcreteColleague2 c2 = new ConcreteColleague2();
c2.SetMediator(mediator);
mediator.RegisterColleagues(c2);

c2.NotifyMediator();

public class ConcreteColleague1 : Colleague
{
	public override void ControlColleague()
	{
		Console.WriteLine("Colleague1 Change");
	}
}

public class ConcreteColleague2 : Colleague
{
	public override void ControlColleague()
	{
		Console.WriteLine("Colleague2 Change");
	}
}

public class ConcreteColleague3 : Colleague
{
	public override void ControlColleague()
	{
		Console.WriteLine("Colleague3 Change");
	}
}

public class ConcreteMediator : Mediator
{
	/// <summary>
	/// 收集各个同事发来的变化,统一逻辑处理
	/// </summary>
	public override void ColleagueChanged()
	{
		//....
		if (colleagues.Count > 0)
		{
			colleagues[0].ControlColleague();
		}
	}
}

public abstract class Mediator
{

	protected List<Colleague> colleagues = new List<Colleague>();

	public void RegisterColleagues(Colleague colleague)
	{
		colleagues.Add(colleague);
	}
	public abstract void ColleagueChanged();

}

public abstract class Colleague
{
	private Mediator mediator;

	public void SetMediator(Mediator mediator)
	{
		this.mediator = mediator;
	}

	public abstract void ControlColleague();

	public void NotifyMediator()
	{
		mediator.ColleagueChanged();
	}
}