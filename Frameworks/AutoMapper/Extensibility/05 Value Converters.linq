<Query Kind="Program">
  <NuGetReference>AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

// Value Converters （值转化）

// Value Converter 是 Type Converter 和 Value Resolver 的合体。
// Type Converter 是全局范围的，因此，无论何时在任何映射中从类型映射到类型，都将使用Type Converter。
// Value Converter 的作用域为单个映射，并接收源对象和目标对象以解析为要映射到目标成员的值。

void Main()
{
	// 在成员属性上使用 Value Converter
	var configuration = new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<Order, OrderDto>()
			.ForMember(d => d.Amount, opt => opt.ConvertUsing(new CurrencyFormatter()));
	});
	
	 var mapper = configuration.CreateMapper();
	 Order order = new Order{Amount = 10};
	 mapper.Map<OrderDto>(order).Dump();

	
	// 也可以映射字段名字不一致的情况。
	configuration = new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<Order, OrderDto>()
			.ForMember(d => d.Amount, opt => opt.ConvertUsing(new CurrencyFormatter(), src => src.OrderAmount));

	});
	
	order .OrderAmount = 100;
	 mapper = configuration.CreateMapper();
	 mapper.Map<OrderDto>(order).Dump();


	// IOC 配置方式。
	configuration = new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<Order, OrderDto>()
			.ForMember(d => d.Amount, opt => opt.ConvertUsing<CurrencyFormatter, decimal>());
	});
	
	
}

public class CurrencyFormatter : IValueConverter<decimal, string>
{
	public string Convert(decimal source, ResolutionContext context)
		=> source.ToString("c");
}

public class Order 
{
	public decimal Amount { get; set; }
	public decimal OrderAmount {get;set;}
}

public class OrderDto 
{
	public string Amount {get;set;}
}


