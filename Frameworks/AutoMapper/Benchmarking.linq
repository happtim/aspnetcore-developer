<Query Kind="Program">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <NuGetReference>BenchmarkDotNet</NuGetReference>
  <Namespace>AutoMapper</Namespace>
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
  <Namespace>BenchmarkDotNet.Order</Namespace>
  <Namespace>BenchmarkDotNet.Running</Namespace>
</Query>


#LINQPad optimize+     // Enable compiler optimizations

void Main()
{
	Util.AutoScrollResults = true;
	BenchmarkRunner.Run<MappingBenchmark>();
}


[MemoryDiagnoser] // 添加内存诊断，分析内存分配
[Orderer(SummaryOrderPolicy.FastestToSlowest)] // 结果按速度排序
[RankColumn]
public class MappingBenchmark
{
    private IMapper _mapper;
    private List<Order> _sourceOrders;
    // 使用不同的数据量进行测试
    [Params(100, 1000)]
    public int N;
    [GlobalSetup]
    public void Setup()
    {
        // 1. 配置 AutoMapper
        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = config.CreateMapper();
        // 2. 创建测试数据
        _sourceOrders = new List<Order>();
        var random = new Random();
        for (int i = 0; i < N; i++)
        {
            _sourceOrders.Add(new Order
            {
                Id = i,
                OrderNumber = $"ORD-{i:D8}",
                TotalAmount = (decimal)(random.NextDouble() * 1000),
                OrderDate = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                CustomerId = Guid.NewGuid(),
                IsShipped = i % 2 == 0,
                BillingAddress = new Address
                {
                    Street = $"{i} Main St",
                    City = "Anytown",
                    PostalCode = "12345"
                },
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, ProductName = "Laptop", Quantity = 1, UnitPrice = 1200.50m },
                    new OrderItem { ProductId = 2, ProductName = "Mouse", Quantity = 1, UnitPrice = 25.00m }
                }
            });
        }
    }
    [Benchmark(Baseline = true)]
    public List<OrderDto> Manual_Mapping()
    {
        var destinationList = new List<OrderDto>(_sourceOrders.Count);
        foreach (var source in _sourceOrders)
        {
            var destination = new OrderDto
            {
                Id = source.Id,
                OrderNumber = source.OrderNumber,
                TotalAmount = source.TotalAmount,
                OrderDate = source.OrderDate,
                CustomerId = source.CustomerId,
                IsShipped = source.IsShipped,
                BillingAddress = new AddressDto // 手动映射嵌套对象
                {
                    Street = source.BillingAddress.Street,
                    City = source.BillingAddress.City,
                    PostalCode = source.BillingAddress.PostalCode
                },
                // 手动映射列表
                Items = new List<OrderItemDto>()
            };
			
			foreach(var orderItem in source.Items)
			{
				destination.Items.Add(new OrderItemDto
				{
					ProductId = orderItem.ProductId,
					ProductName = orderItem.ProductName,
					Quantity = orderItem.Quantity,
					UnitPrice = orderItem.UnitPrice
				});
			}

			destinationList.Add(destination);
        }
        return destinationList;
    }
    
    [Benchmark]
    public List<OrderDto> AutoMapper_Mapping()
    {
        // AutoMapper 一行代码搞定
        return _mapper.Map<List<OrderDto>>(_sourceOrders);
	}
}

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		// 因为属性名和类型完全匹配，所以映射非常简单。
		// AutoMapper 会自动处理嵌套对象和列表的映射。
		CreateMap<Order, OrderDto>();
		CreateMap<Address, AddressDto>();
		CreateMap<OrderItem, OrderItemDto>();
	}
}

// --- 源对象 (Source Objects) ---
/// <summary>
/// 账单地址（源）
/// </summary>
public class Address
{
	public string Street { get; set; }
	public string City { get; set; }
	public string PostalCode { get; set; }
}
/// <summary>
/// 订单项（源）
/// </summary>
public class OrderItem
{
	public int ProductId { get; set; }
	public string ProductName { get; set; }
	public int Quantity { get; set; }
	public decimal UnitPrice { get; set; }
}
/// <summary>
/// 订单实体（源）- 包含多种数据类型
/// </summary>
public class Order
{
	public int Id { get; set; } // int
	public string OrderNumber { get; set; } // string
	public decimal TotalAmount { get; set; } // decimal
	public DateTime OrderDate { get; set; } // DateTime
	public Guid CustomerId { get; set; } // Guid
	public bool IsShipped { get; set; } // bool
	public Address BillingAddress { get; set; } // 嵌套对象
	public List<OrderItem> Items { get; set; } // 对象列表
	public Order()
	{
		Items = new List<OrderItem>();
	}
}
// --- DTO 对象 (Data Transfer Objects) ---
/// <summary>
/// 账单地址 DTO
/// </summary>
public class AddressDto
{
	public string Street { get; set; }
	public string City { get; set; }
	public string PostalCode { get; set; }
}
/// <summary>
/// 订单项 DTO
/// </summary>
public class OrderItemDto
{
	public int ProductId { get; set; }
	public string ProductName { get; set; }
	public int Quantity { get; set; }
	public decimal UnitPrice { get; set; }
}
/// <summary>
/// 订单 DTO
/// </summary>
public class OrderDto
{
	public int Id { get; set; }
	public string OrderNumber { get; set; }
	public decimal TotalAmount { get; set; }
	public DateTime OrderDate { get; set; }
	public Guid CustomerId { get; set; }
	public bool IsShipped { get; set; }
	public AddressDto BillingAddress { get; set; } // 嵌套 DTO
	public List<OrderItemDto> Items { get; set; } // DTO 列表
	public OrderDto()
	{
		Items = new List<OrderItemDto>();
	}
}