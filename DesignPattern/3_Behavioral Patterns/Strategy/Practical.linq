<Query Kind="Statements" />

MovieTicket mt = new MovieTicket();
double originalPrice = 60.0;
double currentPrice;

mt.setPrice(originalPrice);
Console.WriteLine("原始价格:" + originalPrice);

Discount discount = new StudentDiscount();
mt.setDiscount(discount);

currentPrice = mt.getPrice();
Console.WriteLine("折扣价格:" + currentPrice);

public class MovieTicket
{
	private double price;
	private Discount discount;

	public void setPrice(double price)
	{
		this.price = price;
	}

	public void setDiscount(Discount discount)
	{
		this.discount = discount;
	}

	public double getPrice()
	{
		return discount.calculate(this.price);
	}
}


public class VipDiscount : Discount
{

	public override double calculate(double price)
	{
		Console.WriteLine("VIP票:");
		Console.WriteLine("增加积分!");
		return price * 0.5;
	}
}

public class ChildrenDiscount : Discount
{
	public override double calculate(double price)
	{
		Console.WriteLine("学生票:");
		return price - 10;
	}
}


public class StudentDiscount : Discount
{

	public override double calculate(double price)
	{
		Console.WriteLine("学生票:");
		return price * 0.8;
	}
}


public abstract class Discount
{
	public abstract double calculate(double price);
}