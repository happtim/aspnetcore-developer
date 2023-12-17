<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//Flattening (平坦化)


//对象-对象映射的常见用法之一是采用复杂的对象模型并将其展平为更简单的模型。
//在 AutoMapper 中配置源/目标类型对时，配置器会尝试将源类型的属性和方法与目标类型的属性进行匹配。
//如果对于目标类型上的任何属性，源类型上不存在以“Get”为前缀的属性、方法或方法，
//则 AutoMapper 会将目标成员名称拆分为单个单词（根据 PascalCase 约定）。

var customer = new Customer { Name = "George Costanza" };

var order = new Order { Customer = customer };
var bosco = new Product { Name = "Bosco", Price = 4.99m };

order.AddOrderLineItem(bosco, 15);


var configuration = new MapperConfiguration(cfg => { 
	cfg.CreateMap<Order, OrderDto>() ; 
	// LowerUnderscoreNamingConvention
	// PascalCaseNamingConvention
	// ExactMatchNamingConvention
	
	// 使用ExactMatchNamingConvention将此功能禁用。
	//cfg.DestinationMemberNamingConvention = new ExactMatchNamingConvention();
	});

var mapper=  configuration.CreateMapper();


mapper.Map<OrderDto>(order).Dump();


public class OrderDto
{
	// 属性与订单上的 Customer.Name 属性匹配。只要我们正确命名目标属性，我们就不需要配置单个属性匹配。
	public string CustomerName { get; set; }
	//在 OrderDto 类型上，Total 属性与 Order 上的 GetTotal（） 方法匹配。
	public decimal Total { get; set; }
}

public class Order
{
	private readonly IList<OrderLineItem> _orderLineItems = new List<OrderLineItem>();

	public Customer Customer { get; set; }

	public OrderLineItem[] GetOrderLineItems()
	{
		return _orderLineItems.ToArray();
	}

	public void AddOrderLineItem(Product product, int quantity)
	{
		_orderLineItems.Add(new OrderLineItem(product, quantity));
	}

	public decimal GetTotal()
	{
		return _orderLineItems.Sum(li => li.GetTotal());
	}
}

public class Product
{
	public decimal Price { get; set; }
	public string Name { get; set; }
}

public class OrderLineItem
{
	public OrderLineItem(Product product, int quantity)
	{
		Product = product;
		Quantity = quantity;
	}

	public Product Product { get; private set; }
	public int Quantity { get; private set; }

	public decimal GetTotal()
	{
		return Quantity * Product.Price;
	}
}

public class Customer
{
	public string Name { get; set; }
}

