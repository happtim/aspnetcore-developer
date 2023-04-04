<Query Kind="Statements" />



Context context = new Context();
Strategy strategy = new ConcreteStrategyB();
context.setStrategy(strategy);
context.algorithm();

public class Context
{
	private Strategy strategy;

	public void setStrategy(Strategy strategy)
	{
		this.strategy = strategy;
	}

	public void algorithm()
	{
		strategy.algorithm();
	}

}


public class ConcreteStrategyB : Strategy
{

	public override void algorithm()
	{
		Console.WriteLine("ConcreteStrategyB algorithm");
	}
}

public class ConcreteStrategyA : Strategy
{

	public override void algorithm()
	{
		Console.WriteLine("ConcreteStrategyA algorithm");
	}
}

public abstract class Strategy
{
	public abstract void algorithm();
}