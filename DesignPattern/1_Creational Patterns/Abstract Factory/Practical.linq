<Query Kind="Statements" />



KFCFactory factory = new CheapPackageFactory();
KFCDrink drink = factory.CreateDrink();
drink.Dispaly();

KFCFood food = factory.CreateFood();
food.Display();

// 豪华型套餐,包括鸡腿和咖啡
public class LuxuryPackageFactory : KFCFactory
{

	public override KFCDrink CreateDrink()
	{
		return new Coffee();
	}

	public override KFCFood CreateFood()
	{
		return new Chicken();
	}
}

public class Chicken : KFCFood
{
	public override void Display() => Console.WriteLine("鸡腿");
}

public class Coffee : KFCDrink
{
	public override void Dispaly() =>Console.WriteLine("咖啡");
}

// 经济型套餐,包括鸡翅和可乐
public class CheapPackageFactory : KFCFactory
{

	public override KFCDrink CreateDrink()
	{
		return new Coke();
	}

	public override KFCFood CreateFood()
	{
		return new Wings();
	}
}

public class Coke : KFCDrink
{
	public override void Dispaly() => Console.WriteLine("可乐");
}

public class Wings : KFCFood
{
	public override void Display() => Console.WriteLine("鸡翅");
	
}

public abstract class KFCDrink
{
	public abstract void Dispaly();
}

public abstract class KFCFood
{
	public abstract void Display();
}

public abstract class KFCFactory
{
	public abstract KFCFood CreateFood();
	public abstract KFCDrink CreateDrink();
}