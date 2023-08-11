<Query Kind="Statements" />


Receiver receiver = new Receiver();
Command command = new ConcreteCommand(receiver);
Invoker invoker = new Invoker(command);

invoker.call();

public class Invoker
{
	private Command command;
	//构造注入
	public Invoker(Command command)
	{
		this.command = command;
	}

	//设值注入
	public void SetCommand(Command command)
	{
		this.command = command;
	}

	public void call()
	{
		command.execute();
	}
}

public class ConcreteCommand : Command
{
	private Receiver receiver;

	public ConcreteCommand(Receiver receiver)
	{
		this.receiver = receiver;
	}

	public void execute()
	{
		receiver.action();
	}
}

public class Receiver
{
	public void action()
	{
		Console.WriteLine("Receiver action called");
	}
}

public interface Command
{
	void execute();
}