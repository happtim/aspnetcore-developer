<Query Kind="Statements" />


ObjectStructure os = new ObjectStructure();
os.addElement(new ConcreteElementA());
os.addElement(new ConcreteElementB());
os.accept(new ConcreteVisitorA());
os.accept(new ConcreteVisitorB());


public class ObjectStructure
{
	private List<Element> list = new List<Element>();

	public void accept(Visitor visitor)
	{
		foreach (var v in list)
		{
			v.accept(visitor);
		}
	}

	public void addElement(Element element)
	{
		list.Add(element);
	}

	public void removeElement(Element element)
	{
		list.Remove(element);
	}
}

public class ConcreteVisitorB : Visitor
{

	public override void visit(ConcreteElementA elementA)
	{
		Console.WriteLine("ConcreteVisitorB visitor elementA");
	}

	public override void visit(ConcreteElementB elementB)
	{
		Console.WriteLine("ConcreteVisitorB visitor elementB");
	}
}

public class ConcreteVisitorA : Visitor
{
	public override void visit(ConcreteElementA elementA)
	{
		Console.WriteLine("ConcreteVisitorA visitor elementA");
	}

	public override void visit(ConcreteElementB elementB)
	{
		Console.WriteLine("ConcreteVisitorA visitor elementB");
	}
}

public class ConcreteElementA : Element
{

	public override void accept(Visitor visitor)
	{
		visitor.visit(this);
	}
}

public class ConcreteElementB : Element
{

	public override void accept(Visitor visitor)
	{
		visitor.visit(this);
	}
}

public abstract class Element
{
	public abstract void accept(Visitor visitor);
}

public abstract class Visitor
{
	public abstract void visit(ConcreteElementA elementA);
	public abstract void visit(ConcreteElementB elementB);
}