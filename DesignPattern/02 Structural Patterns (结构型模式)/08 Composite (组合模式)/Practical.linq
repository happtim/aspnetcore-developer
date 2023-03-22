<Query Kind="Statements" />

//DotNetty.Buffers.CompositeByteBuffer

Directory rootdir = new Directory("root");
Directory bindir = new Directory("bin");
Directory tmpdir = new Directory("bin");
Directory usrdir = new Directory("usr");
rootdir.Add(bindir);
rootdir.Add(tmpdir);
rootdir.Add(usrdir);
bindir.Add(new File("vi", 10000));
bindir.Add(new File("latex", 20000));
rootdir.printList();

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

	public override void printList(string prefix)
	{
		Console.WriteLine(prefix + "/" + this);
	}
}

public class Directory : Entry
{

	private string name;
	private List<Entry> directory = new List<Entry>();

	public Directory(string name)
	{
		this.name = name;
	}

	public override string getName()
	{
		return name;
	}
	public override Entry Add(Entry entry)
	{
		directory.Add(entry);
		return this;
	}

	public override int getSize()
	{
		int size = 0;
		foreach (var e in directory)
		{
			size += e.getSize();
		}
		return size;
	}

	public override void printList(string prefix)
	{
		Console.WriteLine(prefix + "/" + this);
		foreach (var e in directory)
		{
			e.printList(prefix + "/" + name);
		}
	}
}


public abstract class Entry
{
	public abstract string getName();
	public abstract int getSize();

	public virtual Entry Add(Entry entry)
	{
		throw new NotImplementedException();
	}

	public void printList()
	{
		printList("");
	}

	public abstract void printList(string prefix);

	public override string ToString()
	{
		return getName() + "(" + getSize() + ")";
	}

}