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
// 测试用例3：集合导航属性 + 扩展属性筛选
// =============================================================================
// 场景：查询存在满足条件的标签的产品（标签的 CustomValue > 100）
// 技术点：在集合导航属性中访问扩展属性

Console.WriteLine("=== 【测试用例3】集合导航属性 + 扩展属性筛选 ===");
Console.WriteLine("场景：查询标签的 CustomValue > 100 的产品\n");

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
		.Where(p => p.Tags.Any(t => t.GetProperty<int>("CustomValue") > 100))
		.ToList();
	
	Console.WriteLine($"✓ 直接 LINQ 结果数: {directResult.Count}");
	foreach (var product in directResult)
	{
		var matchingTags = product.Tags
			.Where(t => t.GetProperty<int>("CustomValue") > 100)
			.ToList();
		
		Console.WriteLine($"  └─ {product.Name}");
		foreach (var tag in matchingTags)
		{
			var value = tag.GetProperty<int>("CustomValue");
			Console.WriteLine($"      ├─ Tag: {tag.Name} | CustomValue: {value}");
		}
	}
	
	// Dynamic LINQ 查询实现
	Console.WriteLine("\n✓ Dynamic LINQ 查询:");
	
	// 方式1: 集合导航属性 + 扩展属性查询

	 var parsingConfig = new ParsingConfig
	 {
	     CustomTypeProvider = new DefaultDynamicLinqCustomTypeProvider(
	         ParsingConfig.Default,
	         new[] { typeof(HasExtraPropertiesExtensions) }
	     )
	 };
	var parameters = new object[] { 100 };
	//var whereClause = "Tags.Any(int(ExtraProperties[\"CustomValue\"]) > @0)";
	var whereClause = "Tags.Any(int(HasExtraPropertiesExtensions.GetProperty(it, \"CustomValue\", 0)) > @0)";

	var dynamicResult = products.AsQueryable()
		.Where(parsingConfig,whereClause, parameters)
		.ToList();
	
	Console.WriteLine($"  查询表达式: {whereClause}");
	Console.WriteLine($"  参数: [@0=100]");
	Console.WriteLine($"  结果数: {dynamicResult.Count}");
	foreach (var product in dynamicResult)
	{
		var matchingTags = product.Tags
			.Where(t => t.GetProperty<int>("CustomValue") > 100)
			.ToList();
		
		Console.WriteLine($"  └─ {product.Name}");
		foreach (var tag in matchingTags)
		{
			var value = tag.GetProperty<int>("CustomValue");
			Console.WriteLine($"      ├─ Tag: {tag.Name} | CustomValue: {value}");
		}
	}
	
}
catch (Exception ex)
{
	Console.WriteLine($"✗ 异常: {ex.Message}");
	Console.WriteLine($"  堆栈: {ex.StackTrace}");
}
