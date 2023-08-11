<Query Kind="Statements" />



Manager manager = new Manager();
UnderlinePen upen = new UnderlinePen('~');
MessageBox mbox = new MessageBox('*');
MessageBox sbox = new MessageBox('/');

manager.register("strong message", upen);
manager.register("warning box", mbox);
manager.register("slash box", sbox);

Product p1 = manager.create("strong message");
p1.use("Hello World.");
Product p2 = manager.create("warning box");
p2.use("Hello World.");
Product p3 = manager.create("slash box");
p3.use("Hello World.");

public class Manager
{
	private Dictionary<string, Product> showcase = new Dictionary<string, Product>();
	public void register(string name, Product proto)
	{
		showcase.Add(name, proto);
	}
	public Product create(string protoname)
	{
		Product p = showcase[protoname];
		return (Product)p.Clone();
	}
}

public class MessageBox : Product
{
	private char decochar;
	public MessageBox(char decochar)
	{
		this.decochar = decochar;
	}

	public object Clone()
	{
		return this.MemberwiseClone();
	}

	public void use(string s)
	{
		int length = s.Length;
		for (int i = 0; i < length + 4; i++)
		{
			Console.Write(decochar);
		}
		Console.WriteLine();
		Console.WriteLine(decochar + " " + s + " " + decochar);
		for (int i = 0; i < length + 4; i++)
		{
			Console.Write(decochar);
		}
		Console.WriteLine();
	}
}


public class UnderlinePen : Product
{
	private char ulchar;
	public UnderlinePen(char ulchar)
	{
		this.ulchar = ulchar;
	}

	public object Clone()
	{
		return this.MemberwiseClone();
	}

	public void use(string s)
	{
		int length = s.Length;
		Console.WriteLine("\"" + s + "\"");
		Console.Write(" ");
		for (int i = 0; i < length; i++)
		{
			Console.Write(ulchar);
		}
		Console.WriteLine();
	}
}


public interface Product : ICloneable
{
	void use(string s);
}