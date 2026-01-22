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
// 测试用例5：扩展属性排序
// =============================================================================
// 场景：按照扩展属性 CustomRating 进行降序排序
// 技术点：在 OrderBy 中访问扩展属性

Console.WriteLine("=== 【测试用例5】扩展属性排序 ===");
Console.WriteLine("场景：按照 CustomRating 降序排列\n");

// 初始化测试数据
var products = InitializeProducts();

Console.WriteLine("【初始数据】");
PrintProducts(products);

Console.WriteLine("\n【测试执行】");
Console.WriteLine(new string('-', 80));

try
{
	// 直接 LINQ 排序（参考）
	var directResult = products
		.OrderByDescending(p => p.GetProperty<int>("CustomRating"))
		.ToList();
	
	Console.WriteLine($"✓ 直接 LINQ - 按 CustomRating 降序排列:");
	foreach (var product in directResult)
	{
		var rating = product.GetProperty<int>("CustomRating");
		Console.WriteLine($"  └─ {product.Name} | Rating: {rating}");
	}

	// Dynamic LINQ 排序实现
	Console.WriteLine("\n✓ Dynamic LINQ 排序:");
	
	var parsingConfig = new ParsingConfig
	{
		RestrictOrderByToPropertyOrField = false,
		CustomTypeProvider = new DefaultDynamicLinqCustomTypeProvider(
			 ParsingConfig.Default,
			 new[] { typeof(HasExtraPropertiesExtensions) }
		 )
	};
	
	//var orderByClause = "int(ExtraProperties[\"CustomRating\"]) descending";
	 var orderByClause = "HasExtraPropertiesExtensions.GetProperty(it, \"CustomRating\", null) descending";

	var dynamicResult = products.AsQueryable()
		.OrderBy(parsingConfig,orderByClause)
		.ToList();
	
	Console.WriteLine($"  排序表达式: {orderByClause}");
	Console.WriteLine($"  结果:");
	foreach (var product in dynamicResult)
	{
		var rating = product.GetProperty<int>("CustomRating");
		Console.WriteLine($"  └─ {product.Name} | Rating: {rating}");
	}
	
}
catch (Exception ex)
{
	Console.WriteLine($"✗ 异常: {ex.Message}");
	Console.WriteLine($"  堆栈: {ex.StackTrace}");
}
