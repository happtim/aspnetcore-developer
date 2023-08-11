<Query Kind="Statements" />


var history = new MacroCommand();
var draw = new DrawConsole(history);

mouseDragged();
mouseDragged();
mouseDragged();

Console.WriteLine();

draw.paint();

void mouseDragged()
{
	Command command = new DrawCommand(draw);
	history.append(command);
	command.execute();
}

public class MacroCommand : Command
{
	private Stack<Command> commands = new Stack<Command>();

	public void execute()
	{
		foreach (var command in commands)
		{
			command.execute();
		}
	}

	public void append(Command command)
	{
		commands.Push(command);
	}

	public void undo()
	{
		if (commands.Count > 0)
		{
			commands.Pop();
		}
	}

	public void clear()
	{
		commands.Clear();
	}
}


public class DrawCommand : Command
{

	protected Drawable drawable;

	public DrawCommand(Drawable drawable)
	{
		this.drawable = drawable;
	}

	public void execute()
	{
		drawable.draw();
	}
}

public class DrawConsole : Drawable
{
	private MacroCommand history;

	public DrawConsole(MacroCommand history)
	{
		this.history = history;
	}

	public void paint()
	{
		history.execute();
	}

	public void draw()
	{
		Console.WriteLine("DrawSomeThing");
	}
}

// 命令接受者接口
public interface Drawable
{
	void draw();
}

public interface Command
{
	void execute();
}