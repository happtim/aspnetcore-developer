<Query Kind="Statements" />


//State设计模式将状态的转换逻辑分散在各个状态类中，每个状态类只负责自己状态的逻辑。
//这样可以降低状态间的耦合度，并且方便增加新的状态。

Context context = new Context();

context.Week = WeekEnum.Sunday;
context.changeState();
Console.WriteLine(context.request());

context.Week = WeekEnum.Monday;
context.changeState();
Console.WriteLine(context.request());


class Context
{
	//抽象状态对象的一个引用
	private State state;

	public WeekEnum Week { get; set; }

	public void setState(State state)
	{
		this.state = state;
	}

	public string request()
	{
		//...

		//调用状态对象的业务方法
		return state.handle();

		//...
	}

	public void changeState()
	{
		if (Week == WeekEnum.Sunday)
			this.setState(new SundayState());
		else if (Week == WeekEnum.Monday)
			this.setState(new MondayState());
	}
}

public class SundayState : State
{
	public override string handle()
	{
		return "Sunday";
	}
}

public class MondayState : State
{
	public override string handle()
	{
		return "Monday";
	}
}

public abstract class State
{
	public abstract string handle();
}

public enum WeekEnum
{
	Sunday,
	Monday,
	Tuesday,
	Wednesday,
	Thursday,
	Friday,
	Saturday
}