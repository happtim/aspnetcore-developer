<Query Kind="Statements" />



Caretaker caretaker = new Caretaker();
caretaker.setMemento(new Memento(new Originator() { state = "state1" }));
Originator o = new Originator();
o.restoreMemento(caretaker.getMemento());


// 管理者，它负责保存备忘录
public class Caretaker
{
	private Memento memento;

	public Memento getMemento()
	{
		return memento;
	}

	public void setMemento(Memento memento)
	{
		this.memento = memento;
	}
}


// 备忘录,可以保存原发器内部状态
public class Memento
{
	public string state { get; set; }

	public Memento(Originator o)
	{
		state = o.state;
	}
}


// 原发器,可以创建memento,保存内部状态
public class Originator
{

	public string state { get; set; }

	public Originator() { }

	// 创建一个备忘录对象    
	public Memento createMemento()
	{
		return new Memento(this);
	}

	// 根据备忘录对象恢复原发器状态    
	public void restoreMemento(Memento m)
	{
		state = m.state;
	}

}