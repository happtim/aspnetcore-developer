<Query Kind="Statements" />


Display d1 = new Display(new StringDisplayImpl("Hello China."));
Display d2 = new Display(new StringDisplayImpl("Hello World."));
CountDisplay d3 = new CountDisplay(new StringDisplayImpl("Hello Universe."));

d1.display();
d2.display();
d3.display();

d3.multiDisplay(3);

public class CountDisplay : Display
{
	public CountDisplay(DisplayImpl impl) : base(impl){}

	public void multiDisplay(int times)
	{
		open();
		for (int i = 0; i < times; i++)
		{
			print();
		}
		close();
	}
}

public class Display
{
	private DisplayImpl _impl;
	public Display(DisplayImpl impl)
	{
		this._impl = impl;
	}

	public void open() => _impl.rawOpen();
	
	public void print() => _impl.rawPrint();

	public void close() =>_impl.rawClose();
	
	public void display()
	{
		open();
		print();
		close();
	}
}

public class StringDisplayImpl : DisplayImpl
{
	private string _str;
	private int _width;

	public StringDisplayImpl(string str)
	{
		this._str = str;
		this._width = str.Length;
	}

	public override void rawClose() => printLine();
	
	public override void rawOpen() =>printLine();

	public override void rawPrint() => Console.WriteLine("|" + _str + "|");

	public void printLine()
	{
		Console.Write("+");
		for (int i = 0; i < _width; i++)
		{
			Console.Write("-");
		}
		Console.WriteLine("+");

	}
}

public abstract class DisplayImpl
{
	public abstract void rawOpen();
	public abstract void rawPrint();
	public abstract void rawClose();
}