<Query Kind="Statements" />


IPrint p = new PrintBanner("Hello");
p.printWeak();
p.printStrong();

public class PrintBanner : Banner, IPrint
{
	public PrintBanner(string name) : base(name){}

	public void printStrong()
	{
		showWithParen();
	}

	public void printWeak()
	{
		showWithAStar();
	}
}

public class Banner
{

	private string name;
	public Banner(string name)
	{
		this.name = name;
	}

	public void showWithParen()
	{
		Console.WriteLine("(" + name + ")");
	}

	public void showWithAStar()
	{
		Console.WriteLine("*" + name + "*");
	}
}

public interface IPrint
{
	// 弱化
	void printWeak();

	// 强化
	void printStrong();
}