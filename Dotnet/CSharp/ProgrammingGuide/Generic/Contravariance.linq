<Query Kind="Statements" />

//逆变（Contravariance）
//逆变允许你在泛型接口和委托中使用更通用的类型，而不是更具体的类型。这意味着你可以将父类的对象赋值给子类对象的引用。
//
//逆变的声明方式是使用 in 关键字，这个关键字只能用于输入参数位置（即方法的参数）。

IContravariant<Animal> animalHandler = new AnimalHandler();
IContravariant<Dog> dogHandler = animalHandler;  // 逆变
dogHandler.DoSomething(new Dog());

public class Animal
{
	public string Name { get; set; }
}

public class Dog : Animal
{
	public Dog()
	{
		Name = "Dog";
		Breed = "泰迪";
	}
	public string Breed { get; set; }
}

public interface IContravariant<in T>
{
	void DoSomething(T item);
}

public class AnimalHandler : IContravariant<Animal>
{
	public void DoSomething(Animal animal)
	{
		Console.WriteLine($"Animal name is {animal.Name}");
	}
}