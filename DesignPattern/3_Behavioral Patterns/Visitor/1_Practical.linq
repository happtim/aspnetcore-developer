<Query Kind="Statements" />

Console.WriteLine("making root entries...");
Directory rootdir = new Directory("root");
Directory bindir = new Directory("bin");
Directory usrdri = new Directory("usr");
Directory tmpdir = new Directory("tmp");
Directory homedir = new Directory("home");

rootdir.add(bindir);
rootdir.add(usrdri);
rootdir.add(tmpdir);
rootdir.add(homedir);

bindir.add(new File("vi", 10000));
bindir.add(new File("latex", 20000));
rootdir.accept(new ListVisitor());

Console.WriteLine();
Console.WriteLine("making user entries...");
Directory tim = new Directory("tim");
Directory tom = new Directory("tom");
Directory sam = new Directory("sam");

homedir.add(tim);
homedir.add(tom);
homedir.add(sam);

tim.add(new File("diary.html", 100));
tim.add(new File("composite.java", 200));
tom.add(new File("memo.text", 300));
sam.add(new File("game.doc", 400));
sam.add(new File("junk.mail", 500));

rootdir.accept(new ListVisitor());

public class ListVisitor : Visitor
{

	private string currentDir = "";

	public override void visit(Directory directory)
	{
		Console.WriteLine(currentDir + "/" + directory);

		string saveDir = currentDir;
		currentDir = currentDir + "/" + directory.getName();
		foreach (Entry entry in directory)
		{
			entry.accept(this);
		}
		currentDir = saveDir;

	}

	public override void visit(File file)
	{
		Console.WriteLine(currentDir + "/" + file);

	}

}

public class Directory : Entry, IEnumerable
{

	private string name;
	private List<Entry> dir = new List<Entry>();

	public Directory(string name)
	{
		this.name = name;
	}

	public override void accept(Visitor visitor)
	{
		visitor.visit(this);
	}

	public new Entry add(Entry entry)
	{
		dir.Add(entry);
		return this;
	}

	public override string getName()
	{
		return name;
	}

	public override int getSize()
	{
		int size = 0;
		foreach (var file in dir)
		{
			size += file.getSize();
		}
		return size;
	}

	public IEnumerator GetEnumerator()
	{
		return dir.GetEnumerator();
	}
}

public class File : Entry
{

	private string name;
	private int size;

	public File(string name, int size)
	{
		this.name = name;
		this.size = size;
	}

	public override string getName()
	{
		return name;
	}

	public override int getSize()
	{
		return size;
	}

	public override void accept(Visitor visitor)
	{
		visitor.visit(this);
	}

}

public abstract class Entry : Element
{

	public abstract void accept(Visitor visitor);

	public Entry add(Entry entry)
	{
		throw new NotImplementedException();
	}

	public abstract string getName();

	public abstract int getSize();

	public override string ToString()
	{
		return getName() + " (" + getSize() + ")";
	}


}

public abstract class Visitor
{
	public abstract void visit(File file);
	public abstract void visit(Directory directory);

}

public interface Element
{
	void accept(Visitor visitor);
}