<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp.Scripting</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Scripting</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Scripting</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


"=== 示例 2: 传入复杂类型 ===".Dump();

// 准备复杂输入数据
var globals = new ComplexInputGlobals
{
	Customer = new Customer
	{
		Id = 1,
		Name = "李四",
		Email = "lisi@example.com",
		Address = new Address
		{
			City = "北京",
			Street = "长安街1号",
			ZipCode = "100000"
		}
	},
	Orders = new List<Order>
	{
		new Order { Id = 101, ProductName = "笔记本电脑", Quantity = 1, Price = 5999.99m },
		new Order { Id = 102, ProductName = "鼠标", Quantity = 2, Price = 99.99m },
		new Order { Id = 103, ProductName = "键盘", Quantity = 1, Price = 299.99m }
	}
};

// 脚本：计算订单总金额并格式化客户信息
var script = @"
	var totalAmount = Orders.Sum(o => o.Quantity * o.Price);
	var customerInfo = $""客户: {Customer.Name} ({Customer.Email})"";
	var addressInfo = $""地址: {Customer.Address.City}, {Customer.Address.Street}"";
	var orderCount = Orders.Count;
	
	return new {
		CustomerInfo = customerInfo,
		AddressInfo = addressInfo,
		OrderCount = orderCount,
		TotalAmount = totalAmount,
		FormattedTotal = $""¥{totalAmount:N2}""
	};
";

// 需要提供类型信息
var options = ScriptOptions.Default
	.AddReferences(typeof(Customer).Assembly)
	.AddReferences(typeof(System.Linq.Enumerable).Assembly)
	.AddImports("System.Linq");

try
{
	var result = await CSharpScript.EvaluateAsync(script, options, globals);
	result.Dump("订单分析结果");
}
catch (CompilationErrorException e)
{
	Console.WriteLine("脚本编译失败:");
	Console.WriteLine(string.Join(Environment.NewLine, e.Diagnostics));
}
catch (Exception e)
{
	Console.WriteLine($"脚本执行出错: {e.Message}");
}

// 复杂输入类型的全局变量
public class ComplexInputGlobals
{
	public Customer Customer { get; set; }
	public List<Order> Orders { get; set; }
}

public class Customer
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public Address Address { get; set; }
}

public class Address
{
	public string City { get; set; }
	public string Street { get; set; }
	public string ZipCode { get; set; }
}

public class Order
{
	public int Id { get; set; }
	public string ProductName { get; set; }
	public int Quantity { get; set; }
	public decimal Price { get; set; }
}
