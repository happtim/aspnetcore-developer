<Query Kind="Statements" />

//协变（Covariance）
//协变允许你在泛型接口和委托中使用更具体的类型，而不是更通用的类型。这意味着你可以将子类的对象赋值给父类对象的引用。
//
//协变的声明方式是使用 out 关键字，这个关键字只能用于泛型接口和泛型委托，并必须用于方法的返回类型（也就是说，它只能作为输出）。


ICovariant<Dog> dogProvider = new DogProvider();
ICovariant<Animal> animalProvider = dogProvider; // 协变
Animal animal = animalProvider.GetItem();
animal.Dump();


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

public interface ICovariant<out T>
{
	T GetItem();
}

public class AnimalProvider : ICovariant<Animal>
{
	public Animal GetItem()
	{
		return new Animal();
	}
}

public class DogProvider : ICovariant<Dog>
{
	public Dog GetItem()
	{
		return new Dog();
	}
}