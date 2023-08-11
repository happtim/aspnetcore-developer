<Query Kind="Statements" />


NumberGenerator generator = new RandomNumberGenerator();
Observer observer1 = new GraphObserver();
Observer observer2 = new DigitObserver();

generator.addObserver(observer1);
generator.addObserver(observer2);

generator.execute();

public class GraphObserver : Observer
{
	public void update(NumberGenerator numberGenerator)
	{
		Console.Write("GraphObserver:");
		int count = numberGenerator.Number;
		for (int i = 0; i < count; i++)
		{
			Console.Write("*");
		}
		Console.WriteLine();
		Thread.Sleep(100);

	}
}

public class DigitObserver : Observer
{
	public void update(NumberGenerator numberGenerator)
	{
		Console.WriteLine("DigitObserver:" + numberGenerator.Number);
		Thread.Sleep(100);
	}
}

public interface Observer
{
	void update(NumberGenerator numberGenerator);
}

public class RandomNumberGenerator : NumberGenerator
{

	private int number;

	public override int Number { get { return number; } }

	public override void execute()
	{

		for (int i = 0; i < 10; i++)
		{
			number = (new Random()).Next(50);
			notifyObservers();
		}
	}

}

public abstract class NumberGenerator
{
	private List<Observer> observers = new List<Observer>();

	public void addObserver(Observer observer)
	{
		observers.Add(observer);
	}

	public void deleteObserver(Observer observer)
	{
		observers.Remove(observer);
	}

	public void notifyObservers()
	{
		foreach (var observer in observers)
		{
			observer.update(this);
		}
	}

	public virtual int Number { get; }
	public abstract void execute();

}