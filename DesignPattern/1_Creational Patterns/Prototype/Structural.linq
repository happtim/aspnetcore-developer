<Query Kind="Statements" />


Prototype p1 = new ConcretePrototype("Tim");
Prototype p2 = p1.CreateClone();
p2.Call("sam");

public class ConcretePrototype : Prototype
{
	private string name;
	public ConcretePrototype(string name)
	{
		this.name = name;
	}
	public string Name { get { return name; } }

	public Prototype CreateClone()
	{
		Prototype obj = (Prototype)this.MemberwiseClone();
		return obj;
	}

	public void Call(string someone)
	{
		Console.WriteLine(this.name + " Call " + someone);
	}
}


public interface Prototype
{
	Prototype CreateClone();
	void Call(string someone);
}