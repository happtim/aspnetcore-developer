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
// 测试用例1：基础扩展属性查询
// =============================================================================
// 场景：查询 CustomColor = 'Red' 的产品
// 技术点：使用 ExtraProperties 字典直接访问扩展属性

Console.WriteLine("=== 【测试用例1】基础扩展属性查询 ===");
Console.WriteLine("场景：查询 CustomColor = 'Red' 的产品\n");

// 初始化测试数据
var products = InitializeProducts();

Console.WriteLine("【初始数据】");
PrintProducts(products);

Console.WriteLine("\n【测试执行】");
Console.WriteLine(new string('-', 80));

try
{
	var query = products.AsQueryable();
	
	// 直接 LINQ 查询（参考）
	var directResult = products
		.Where(p => p.GetProperty<string>("CustomColor") == "Red")
		.ToList();
	
	Console.WriteLine($"✓ 直接 LINQ 结果数: {directResult.Count}");
	foreach (var product in directResult)
	{
		var color = product.GetProperty<string>("CustomColor");
		Console.WriteLine($"  └─ {product.Name} | Color: {color}");
	}
	
	// Dynamic LINQ 查询实现
	Console.WriteLine("\n✓ Dynamic LINQ 查询:");
	
	// 使用 HasExtraPropertiesExtensions.GetProperty (需要配置 ParsingConfig)
	 var parsingConfig = new ParsingConfig
	 {
	     CustomTypeProvider = new DefaultDynamicLinqCustomTypeProvider(
	         ParsingConfig.Default,
	         new[] { typeof(HasExtraPropertiesExtensions) }
	     )
	 };
	 
	//var whereClause = "string(ExtraProperties[\"CustomColor\"]) == @0";
	 var whereClause = "HasExtraPropertiesExtensions.GetProperty(it, \"CustomColor\", \"\") == @0";
	 var parameters = new object[] { "Red" };
	 var dynamicResult = query.Where(parsingConfig, whereClause, parameters).ToList();
	
	Console.WriteLine($"  查询表达式: {whereClause}");
	Console.WriteLine($"  参数: [@0=\"Red\"]");
	Console.WriteLine($"  结果数: {dynamicResult.Count}");
	foreach (var product in dynamicResult)
	{
		var color = product.GetProperty<string>("CustomColor");
		Console.WriteLine($"  └─ {product.Name} | Color: {color}");
	}
	
}
catch (Exception ex)
{
	Console.WriteLine($"✗ 异常: {ex.Message}");
	Console.WriteLine($"  堆栈: {ex.StackTrace}");
}
