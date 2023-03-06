<Query Kind="Statements" />

Display b1 = new StringDisplay("Hello World!");
Display b2 = new SideBorder(b1, '#');
Display b3 = new FullBorder(b2);
b1.show();
b2.show();
b3.show();


public class StringDisplay : Display
{
	private string name;

	public StringDisplay(string name)
	{
		this.name = name;
	}

	public override int getColumns()
	{
		return name.Length;
	}

	public override int getRows()
	{
		return 1;
	}

	public override string getRowText(int row)
	{
		if (row == 0)
			return name;
		else
			return null;
	}
}

public class SideBorder : Border
{

	private char borderChar;

	public SideBorder(Display display, char ch) : base(display)
	{
		borderChar = ch;
	}

	public override int getColumns()
	{
		return 1 + dispaly.getColumns() + 1;
	}

	public override int getRows()
	{
		return dispaly.getRows();
	}

	public override string getRowText(int row)
	{
		return borderChar + dispaly.getRowText(row) + borderChar;
	}
}

public class FullBorder : Border
{

	public FullBorder(Display display) : base(display){}

	public override int getColumns()
	{
		return 1 + dispaly.getColumns() + 1;
	}

	public override int getRows()
	{
		return 1 + dispaly.getRows() + 1;
	}

	public override string getRowText(int row)
	{
		if (row == 0)
			return "+" + makeLine('-', dispaly.getColumns()) + "+";
		else if (row == dispaly.getRows() + 1)
			return "+" + makeLine('-', dispaly.getColumns()) + "+";
		else
			return "|" + dispaly.getRowText(row - 1) + "|";
	}

	private string makeLine(char ch, int count)
	{
		string str = "";
		for (int i = 0; i < count; i++)
		{
			str += ch;
		}
		return str;
	}
}

public abstract class Border : Display
{

	protected Display dispaly;

	protected Border(Display display)
	{
		this.dispaly = display;
	}
}

public abstract class Display
{

	public abstract int getColumns();
	public abstract int getRows();
	public abstract string getRowText(int row);

	public void show()
	{
		for (int i = 0; i < getRows(); i++)
		{
			Console.WriteLine(getRowText(i));
		}
	}
}