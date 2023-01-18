<Query Kind="Statements" />


Builder builder = new ConcreteBuilder();
Director director = new Director(builder);
Product product = director.Construct();
product.Dump();

public class Director
{
	private Builder _builder;
	public Director(Builder builder)
	{
		this._builder = builder;
	}

	public Product Construct()
	{
		_builder.buildPart1();
		_builder.buildPart2();
		_builder.buildPart3();
		return _builder.getResult();
	}

}

public class ConcreteBuilder : Builder
{

	public override void buildPart1()
	{
		this.product.Part1 = "build Part1";
	}

	public override void buildPart2()
	{
		this.product.Part2 = "build Part2";
	}

	public override void buildPart3()
	{
		this.product.Part3 = "build Part3";
	}

}

public abstract class Builder
{
	protected Product product = new Product();

	public abstract void buildPart1();
	public abstract void buildPart2();
	public abstract void buildPart3();
	
	public Product getResult()
	{
		return product;
	}

}

public class Product
{
	public string Part1;
	public string Part2;
	public string Part3;
}