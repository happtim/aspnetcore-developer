<Query Kind="Program">
  <NuGetReference>AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

// Reverse Mapping and Unflattening （反向映射和还原）

void Main()
{
	//反向映射。
	var configuration = new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<Order, OrderDto>()
		   .ReverseMap();
	});

	var mapper = configuration.CreateMapper();
	
	
	var customer = new Customer
	{
		Name = "Bob"
	};

	var order = new Order
	{
		Customer = customer,
		Total = 15.8m
	};
	
	var orderDto = mapper.Map<Order, OrderDto>(order);
	orderDto.Dump();

	orderDto.CustomerName = "Joe";
	mapper.Map(orderDto, order);
	order.Dump();


	//Customizing reverse mapping （自定义反向映射）
	configuration = new MapperConfiguration(cfg =>
    {
   		cfg.DestinationMemberNamingConvention = new ExactMatchNamingConvention();
	   	cfg.CreateMap<Order, OrderDto>()
	    	.ForMember(d => d.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
			.ReverseMap()
			//一般默认不用使用ForPath. 只有在反向映射时，目标类型路径下属性不一致时才使用。
			.ForPath(s => s.Customer.Name , opt => opt.MapFrom(dest => dest.CustomerName))
			// 如果不想使用还原对象。Ignore参数
			//.ForPath(s => s.Customer.Name , opt => opt.Ignore())
			;
    });
   
   mapper = configuration.CreateMapper();
   orderDto = mapper.Map<Order, OrderDto>(order);
   orderDto.Dump();

	orderDto.CustomerName = "Bob";
	mapper.Map(orderDto, order);
	order.Dump();

}

public class Order
{
	public decimal Total { get; set; }
	public Customer Customer { get; set; }
}

public class Customer
{
	public string Name { get; set; }
}

public class OrderDto
{
	public decimal Total { get; set; }
	public string CustomerName { get; set; }
}