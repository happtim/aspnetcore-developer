<Query Kind="Statements" />


Gamer gamer = new Gamer(100);
Memento memento = gamer.createMemento();

for (int i = 0; i < 100; i++)
{
	Console.WriteLine("---------------- " + i);
	Console.WriteLine("当前状态:" + gamer);

	gamer.bet();

	Console.WriteLine("所持有金额" + gamer.Money + "元");
	if (gamer.Money > memento.Money)
	{
		Console.WriteLine("持有的金额增加了许多,所以保存了游戏的当前状态");
		memento = gamer.createMemento();
	}
	else if (gamer.Money < memento.Money / 2)
	{
		Console.WriteLine("持有的金额减少了许多,所以恢复了游戏的之前状态");
		gamer.restoreMemento(memento);
	}

	Thread.Sleep(100);
	Console.WriteLine();
}

public class Gamer
{
	private int money;
	private List<string> fruits = new List<string>();
	private Random random = new Random();
	private static string[] fruitsname = { "苹果", "葡萄", "香蕉", "橘子" };

	public Gamer(int money)
	{
		this.money = money;
	}

	public int Money { get { return money; } }

	public void bet()
	{
		int dice = random.Next(6) + 1;
		if (dice == 1)
		{
			money += 100;
			Console.WriteLine("所持有的金钱增加了...");
		}
		else if (dice == 2)
		{
			money /= 2;
			Console.WriteLine("所持有的金钱减半了...");
		}
		else if (dice == 6)
		{
			string f = getFruit();
			Console.WriteLine("获得水果(" + f + ")");
			fruits.Add(f);
		}
		else
		{
			Console.WriteLine("什么都没有发生");
		}
	}

	public Memento createMemento()
	{
		Memento m = new Memento(money);
		foreach (var f in fruits)
		{
			if (f.StartsWith("好吃的"))
			{
				m.AddFruit(f);
			}
		}
		return m;
	}

	public void restoreMemento(Memento memento)
	{
		this.money = memento.Money;
		this.fruits = memento.GetFruits();
	}

	public override string ToString()
	{
		return "[money = " + money + ", fruits = " + fruits + "]";
	}

	private string getFruit()
	{
		string prefix = "";
		if (random.NextDouble() > 0.5)
		{
			prefix = "好吃的";
		}
		return prefix + fruitsname[random.Next(fruitsname.Length)];
	}


}

public class Memento
{
	private int money = 0;
	private List<string> fruits = new List<string>();

	public int Money { get { return money; } }

	public Memento(int money)
	{
		this.money = money;
	}

	public void AddFruit(string fruit)
	{
		fruits.Add(fruit);
	}

	public List<string> GetFruits()
	{
		return fruits;
	}

}