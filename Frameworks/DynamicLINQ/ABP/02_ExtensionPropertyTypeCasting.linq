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
// 测试用例2：扩展属性类型转换查询
// =============================================================================
// 场景：查询转换为 int 类型的 CustomRating > 4 的产品
// 技术点：在 Dynamic LINQ 中对扩展属性进行类型转换

Console.WriteLine("=== 【测试用例2】扩展属性类型转换查询 ===");
Console.WriteLine("场景：查询 int 类型的 CustomRating > 4 的产品\n");

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
		.Where(p => p.GetProperty<int>("CustomRating") > 4)
		.ToList();
	
	Console.WriteLine($"✓ 直接 LINQ 结果数: {directResult.Count}");
	foreach (var product in directResult)
	{
		var rating = product.GetProperty<int>("CustomRating");
		Console.WriteLine($"  └─ {product.Name} | Rating: {rating}");
	}
	
	// Dynamic LINQ 查询实现
	Console.WriteLine("\n✓ Dynamic LINQ 查询:");
	
	var parsingConfig = new ParsingConfig
	{
		CustomTypeProvider = new DefaultDynamicLinqCustomTypeProvider(
		 	ParsingConfig.Default,
		 	new[] { typeof(HasExtraPropertiesExtensions) }
		)
	};
	 
	//var whereClause = "int(ExtraProperties[\"CustomRating\"]) > @0";
	 var whereClause = "int(HasExtraPropertiesExtensions.GetProperty(it, \"CustomRating\", 0)) > @0";
	var parameters = new object[] { 4 };
	var dynamicResult = products.AsQueryable().Where(parsingConfig, whereClause, parameters).ToList();
	
	Console.WriteLine($"  查询表达式: {whereClause}");
	Console.WriteLine($"  参数: [@0=4]");
	Console.WriteLine($"  结果数: {dynamicResult.Count}");
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
