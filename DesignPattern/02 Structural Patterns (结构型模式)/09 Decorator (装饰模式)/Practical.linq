<Query Kind="Statements" />



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