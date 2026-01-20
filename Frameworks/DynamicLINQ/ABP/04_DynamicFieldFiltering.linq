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
// 测试用例4：动态字段筛选 - 字符串拼接
// =============================================================================
// 场景：根据多个条件动态拼接查询表达式
// 技术点：字符串拼接方式构建复杂的 Dynamic LINQ 查询

Console.WriteLine("=== 【测试用例4】动态字段筛选 - 字符串拼接 ===");
Console.WriteLine("场景：Active=true AND CustomColor='Red' AND CustomRating>3\n");

// 初始化测试数据
var products = InitializeProducts();

Console.WriteLine("【初始数据】");
PrintProducts(products);

Console.WriteLine("\n【测试执行】");
Console.WriteLine(new string('-', 80));

try
{
	// 直接 LINQ 查询（参考实现）
	var directResult = products
		.Where(p => 
			p.Active && 
			p.GetProperty<string>("CustomColor") == "Red" &&
			p.GetProperty<int>("CustomRating") > 3
		)
		.ToList();
	
	Console.WriteLine($"✓ 直接 LINQ 结果数: {directResult.Count}");
	foreach (var product in directResult)
	{
		var color = product.GetProperty<string>("CustomColor");
		var rating = product.GetProperty<int>("CustomRating");
		Console.WriteLine($"  └─ {product.Name} | Color: {color} | Rating: {rating}");
	}
	
	// Dynamic LINQ 查询实现 - 字符串拼接方式
	Console.WriteLine("\n✓ Dynamic LINQ 查询 - 字符串拼接:");
	
	// 方式1: 直接拼接完整的查询字符串
	var conditions = new List<string>();
	var parameters = new List<object>();
	
	// 添加普通属性条件
	conditions.Add($"Active == @{parameters.Count}");
	parameters.Add(true);
	
	// 添加扩展属性条件 - CustomColor
	conditions.Add($"string(ExtraProperties[\"CustomColor\"]) == @{parameters.Count}");
	parameters.Add("Red");
	
	// 添加扩展属性条件 - CustomRating
	conditions.Add($"int(ExtraProperties[\"CustomRating\"]) > @{parameters.Count}");
	parameters.Add(3);

	// 拼接所有条件
	var whereClause = string.Join(" and ", conditions);

	var dynamicResult = products.AsQueryable()
		.Where(whereClause, parameters.ToArray())
		.ToList();

	// 方式2: 使用 HasExtraPropertiesExtensions.GetProperty (需要配置 ParsingConfig)
	 //var parsingConfig = new ParsingConfig
	 //{
	 //    CustomTypeProvider = new DefaultDynamicLinqCustomTypeProvider(
	 //        ParsingConfig.Default,
	 //        new[] { typeof(HasExtraPropertiesExtensions) }
	 //    )
	 //};
	 //var conditions2 = new List<string>();
	 //var parameters2 = new List<object>();
	 //conditions2.Add($"Active == @{parameters2.Count}");
	 //parameters2.Add(true);
	 //conditions2.Add($"HasExtraPropertiesExtensions.GetProperty(it, \"CustomColor\", \"\") == @{parameters2.Count}");
	 //parameters2.Add("Red");
	 //conditions2.Add($"int(HasExtraPropertiesExtensions.GetProperty(it, \"CustomRating\", 0)) > @{parameters2.Count}");
	 //parameters2.Add(3);
	 //var whereClause2 = string.Join(" and ", conditions2);
	 //var dynamicResult = products.AsQueryable().Where(parsingConfig, whereClause2, parameters2.ToArray()).ToList();

	
	Console.WriteLine($"  查询表达式: {whereClause}");
	Console.WriteLine($"  参数: [@0=true, @1=\"Red\", @2=3]");
	Console.WriteLine($"  结果数: {dynamicResult.Count}");
	foreach (var product in dynamicResult)
	{
		var color = product.GetProperty<string>("CustomColor");
		var rating = product.GetProperty<int>("CustomRating");
		Console.WriteLine($"  └─ {product.Name} | Color: {color} | Rating: {rating}");
	}
	
}
catch (Exception ex)
{
	Console.WriteLine($"✗ 异常: {ex.Message}");
	Console.WriteLine($"  堆栈: {ex.StackTrace}");
}
