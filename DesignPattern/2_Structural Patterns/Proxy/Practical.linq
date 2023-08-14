<Query Kind="Statements" />


//Tim 的打印机
Printable p = new PrinterProxy("Tim");
//Tom 打印机
p.SetPrinterName("Tom");
p.Print("Hello World!");


public class PrinterProxy : Printable
{

	private string name;
	private Printer real;

	public PrinterProxy(string name)
	{
		this.name = name;
	}

	public string GetPrinterName()
	{
		return this.name;
	}

	public void SetPrinterName(string name)
	{
		if (real != null)
		{
			real.SetPrinterName(name);
		}
		this.name = name;
	}

	public void Print(string something)
	{
		Realize();
		real.Print(something);
	}

	public void Realize()
	{
		if (real == null)
		{
			real = new Printer(name);
		}
	}
}


public interface Printable
{
	void SetPrinterName(string name);
	string GetPrinterName();
	void Print(string something);
}

public class Printer : Printable
{

	private string name;

	public Printer()
	{
		HeavyJob("Printer Init Begin.");
	}

	public Printer(string name)
	{
		this.name = name;
		HeavyJob("Printer Init Begin.");
	}

	private void HeavyJob(string msg)
	{
		Console.WriteLine(msg);
		for (int i = 0; i < 5; i++)
		{
			Thread.Sleep(1000);
			Console.Write(".");
		}
		Console.WriteLine("End Init.");

	}

	public string GetPrinterName()
	{
		return name;
	}

	public void Print(string something)
	{
		Console.WriteLine("====" + name + " Use Printer ====");
		Console.WriteLine(something);
	}

	public void SetPrinterName(string name)
	{
		this.name = name;
	}
}