<Query Kind="Program">
  <NuGetReference>AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//Flattening (平坦化)

void Main()
{
	//对象-对象映射的常见用法之一是采用复杂的对象模型并将其展平为更简单的模型。
	//AutoMapper 默认将共有属性/字段进行映射。以及源类型上Get前缀的方法和目标类型属性，
	//如果上述两种没有匹配， 则尝试将目标类型成员名称按照（ PascalCase 约定）分拆来匹配源。

	var customer = new Customer
	{
		Name = "George Costanza"
	};
	var order = new Order
	{
		Customer = customer
	};
	var bosco = new Product
	{
		Name = "Bosco",
		Price = 4.99m
	};
	order.AddOrderLineItem(bosco, 15);


	var configuration = new MapperConfiguration(cfg => { 
		cfg.CreateMap<Order, OrderDto>() ; 
		// LowerUnderscoreNamingConvention
		// PascalCaseNamingConvention
		// ExactMatchNamingConvention
		
		// 使用ExactMatchNamingConvention将此功能禁用。
		cfg.DestinationMemberNamingConvention = new ExactMatchNamingConvention();
		});
	
	var mapper=  configuration.CreateMapper();
	//在 OrderDto 类型上，Total 属性与 Order 上的 GetTotal（） 方法匹配。
	// 属性与订单上的 Customer.Name 属性匹配。只要我们正确命名目标属性，我们就不需要配置单个属性匹配。
	mapper.Map<OrderDto>(order).Dump();


	//IncludeMembers 
	// 你可以直接将子类型和目标类型进行映射。 然后就可以通过IncludeMembers方法 子对象的成员映射到目标对象。
	// 这样的一个好处就是可以用重用子类型的映射到目标类型。
	configuration = new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<Source, Destination>().IncludeMembers(s => s.InnerSource, s => s.OtherInnerSource)
		//IncludeMembers函数的参数顺序是对映射结果有相关的。优先匹配参数结果的第一个出现的映射。
		
		//cfg.CreateMap<Source, Destination>().IncludeMembers(s => s.OtherInnerSource, s => s.InnerSource);
		//cfg.CreateMap<Source, Destination>();
		cfg.CreateMap<InnerSource, Destination>(MemberList.None);
		cfg.CreateMap<OtherInnerSource, Destination>()
			.ForPath(destination	 => destination, member => member.MapFrom(source => source));
	});
	
	mapper = configuration.CreateMapper();

	var source = new Source
	{
		Name = "name_Source",
		InnerSource = new InnerSource { Description = "description_InnerSource", Name = "name_InnerSource" },
		OtherInnerSource = new OtherInnerSource { Title = "title_OtherInnerSource", Name = "name_OtherInnerSource" ,Description = "description_OtherInnerSource"}
	};
	
	var destination = mapper.Map<Destination>(source);
	
	destination.Dump();
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

public class OrderDto
{
	public string CustomerName { get; set; }
	public decimal Total { get; set; }
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


class Source
{
	public string Name { get; set; }
	public InnerSource InnerSource { get; set; }
	public OtherInnerSource OtherInnerSource { get; set; }
}
class InnerSource
{
	public string Name { get; set; }
	public string Description { get; set; }
}
class OtherInnerSource
{
	public string Name { get; set; }
	public string Description { get; set; }
	public string Title { get; set; }
}
class Destination
{
	public string Name { get; set; }
	public string Description { get; set; }
	public string Title { get; set; }
}
