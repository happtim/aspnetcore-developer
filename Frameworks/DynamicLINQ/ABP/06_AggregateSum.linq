<Query Kind="Statements">
  <NuGetReference>System.Linq.Dynamic.Core</NuGetReference>
  <NuGetReference Version="8.3.4">Volo.Abp.ObjectExtending</NuGetReference>
  <Namespace>System.Linq.Dynamic.Core</Namespace>
  <Namespace>Volo.Abp.Data</Namespace>
  <Namespace>Volo.Abp.ObjectExtending</Namespace>
  <Namespace>System.Linq.Dynamic.Core.CustomTypeProviders</Namespace>
</Query>

#load ".\Common_Data.linq"

// =============================================================================
// 测试用例6：聚合函数 - Sum
// =============================================================================
// 场景：对每个产品的标签扩展属性 CustomValue 进行求和，并筛选总和 > 150 的产品
// 技术点：在 Dynamic LINQ 中使用聚合函数访问集合的扩展属性

Console.WriteLine("=== 【测试用例6】聚合函数 - Sum ===");
Console.WriteLine("场景：查询标签 CustomValue 总和 > 150 的产品\n");

// 初始化测试数据
var products = InitializeProducts();

Console.WriteLine("【初始数据】");
PrintProducts(products);

Console.WriteLine("\n【测试执行】");
Console.WriteLine(new string('-', 80));

try
{
	// 直接 LINQ 查询（参考）
	var directResult = products
		.Where(p => p.Tags.Sum(t => t.GetProperty<int>("CustomValue")) > 150)
		.OrderByDescending(p => p.Tags.Sum(t => t.GetProperty<int>("CustomValue")))
		.ToList();
	
	Console.WriteLine($"✓ 直接 LINQ 结果数: {directResult.Count}");
	foreach (var product in directResult)
	{
		var totalValue = product.Tags.Sum(t => t.GetProperty<int>("CustomValue"));
		Console.WriteLine($"  └─ {product.Name} | 标签总和: {totalValue}");
		foreach (var tag in product.Tags)
		{
			var value = tag.GetProperty<int>("CustomValue");
			Console.WriteLine($"      ├─ {tag.Name}: {value}");
		}
	}

	// Dynamic LINQ 查询实现
	Console.WriteLine("\n✓ Dynamic LINQ 查询:");
	
	var parsingConfig = new ParsingConfig
	{
		RestrictOrderByToPropertyOrField = false,
		CustomTypeProvider = new DefaultDynamicLinqCustomTypeProvider(
		 ParsingConfig.Default,
		 new[] { typeof(HasExtraPropertiesExtensions) }
	 )
	};
	
	// 方式1: 使用 Sum 聚合函数访问扩展属性
	var whereClause = "Tags.Sum(HasExtraPropertiesExtensions.GetProperty(it, \"CustomRating\", null)) > @0";
	var parameters = new object[] { 150 };
	
	var dynamicResult = products.AsQueryable()
		.Where(parsingConfig,whereClause, parameters)
		.ToList();
	
	Console.WriteLine($"  查询表达式: {whereClause}");
	Console.WriteLine($"  参数: [@0=150]");
	Console.WriteLine($"  结果数: {dynamicResult.Count}");
	foreach (var product in dynamicResult)
	{
		var totalValue = product.Tags.Sum(t => t.GetProperty<int>("CustomValue"));
		Console.WriteLine($"  └─ {product.Name} | 标签总和: {totalValue}");
		foreach (var tag in product.Tags)
		{
			var value = tag.GetProperty<int>("CustomValue");
			Console.WriteLine($"      ├─ {tag.Name}: {value}");
		}
	}

	// 额外测试：按照聚合结果排序
	Console.WriteLine("\n✓ Dynamic LINQ 排序 - 按标签总和降序:");

	// 方式2: 直接访问 ExtraProperties
	var orderByClause = "Tags.Sum(int(ExtraProperties.CustomValue)) descending";
	
	var sortedResult = products.AsQueryable()
		.OrderBy(parsingConfig,orderByClause)
		.ToList();
	
	Console.WriteLine($"  排序表达式: {orderByClause}");
	Console.WriteLine($"  结果:");
	foreach (var product in sortedResult)
	{
		var totalValue = product.Tags.Sum(t => t.GetProperty<int>("CustomValue"));
		Console.WriteLine($"  └─ {product.Name} | 标签总和: {totalValue}");
	}
	
}
catch (Exception ex)
{
	Console.WriteLine($"✗ 异常: {ex.Message}");
	Console.WriteLine($"  堆栈: {ex.StackTrace}");
}
