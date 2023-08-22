<Query Kind="Statements" />

//PipelineNet

//Volo.Abp.Settings.SettingProvider.GetOrNullValueFromProvidersAsync

Handler h1 = new ConcreteHandler1();
Handler h2 = new ConcreteHandler2();

h2.setNext(h1);

h2.resolve("some thing");

public class ConcreteHandler1 : Handler
{
	public override bool request(string someTrouble)
	{
		Console.WriteLine("ConcreteHandler1 Resolve Truoble");
		return true;
	}
}
public class ConcreteHandler2 : Handler
{

	public override bool request(string someTrouble)
	{
		Console.WriteLine("ConcreteHandler2 Can't Resolve Truoble");
		return false;
	}
}


public abstract class Handler
{

	private Handler next;

	public void setNext(Handler handler)
	{
		next = handler;
	}

	public void resolve(string someTrouble)
	{
		if (request(someTrouble))
		{
			//done
		}
		else
		{
			next.request(someTrouble);
		}
	}

	public abstract bool request(string someTrouble);
}