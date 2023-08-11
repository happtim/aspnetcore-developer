<Query Kind="Statements" />


Object[] objArray = { "One", "Two", "Three", "Four", "Five", "Six" };
//创建聚合对象
Aggregate agg = new ConcreteAggregate(objArray);
//循环输出聚合对象中的值
Iterator it = agg.createIterator();
while (!it.isDone())
{
	it.currentItem().Dump();
	it.next();
}

public class ConcreteAggregate: Aggregate
{

	private Object[] objArray = null;

	public ConcreteAggregate(Object[] objArray)
	{
		this.objArray = objArray;
	}

	public override Iterator createIterator()
	{
		return new ConcreteIterator(this);
	}

	public Object getElement(int index)
	{

		if (index < objArray.Length)
		{
			return objArray[index];
		}
		else
		{
			return null;
		}
	}

	public int size()
	{
		return objArray.Length;
	}
}

public class ConcreteIterator : Iterator
{
	//持有被迭代的具体的聚合对象
	private ConcreteAggregate agg;
	//内部索引，记录当前迭代到的索引位置
	private int index = 0;
	//记录当前聚集对象的大小
	private int size = 0;

	public ConcreteIterator(ConcreteAggregate agg)
	{
		this.agg = agg;
		this.size = agg.size();
		index = 0;
	}

	public Object currentItem()
	{
		return agg.getElement(index);
	}

	public void first()
	{
		index = 0;
	}


	public bool isDone()
	{
		return (index >= size);
	}


	public void next()
	{

		if (index < size)
		{
			index++;
		}
	}

}

public abstract class Aggregate
{
	public abstract Iterator createIterator();
}


public interface Iterator
{
	/**
     * 迭代方法：移动到第一个元素
     */
	public void first();
	/**
     * 迭代方法：移动到下一个元素
     */
	public void next();
	/**
     * 迭代方法：是否为最后一个元素
     */
	public bool isDone();
	/**
     * 迭代方法：返还当前元素
     */
	public Object currentItem();
}