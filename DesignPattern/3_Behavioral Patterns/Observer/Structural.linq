<Query Kind="Statements" />

//Volo.Abp.EventBus

Subject subject = new ConcreteSubject();
Observer observer = new ConcreteObserver();
Observer observer2 = new ConcreteObserver2();
subject.addObserver(observer);
subject.addObserver(observer2);
subject.getSubjectStatus();

public class ConcreteObserver2 : Observer
{
	public override void update(Subject subject)
	{
		Console.WriteLine("Observer 2 Watch subject Status Changed");
	}
}


public class ConcreteObserver : Observer
{

	public override void update(Subject subject)
	{
		Console.WriteLine("Observer Wathch subject Status Changed");
	}
}

public class ConcreteSubject : Subject
{

	public override void getSubjectStatus()
	{
		notifyObservers();
	}
}

public abstract class Observer
{
	public abstract void update(Subject subject);
}

public abstract class Subject
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

	public abstract void getSubjectStatus();

}