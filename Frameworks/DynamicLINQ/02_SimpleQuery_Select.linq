<Query Kind="Statements">
  <NuGetReference>System.Linq.Dynamic.Core</NuGetReference>
  <Namespace>System.Linq.Dynamic.Core</Namespace>
</Query>

// ============================================
// Dynamic LINQ - Simple Query - SELECT 示例
// ============================================
// 此示例演示了 Select 子句的各种用法
// 包括：选择全部属性、选择部分属性、投影到匿名类型

Console.WriteLine("=== Dynamic LINQ Select 查询示例 ===\n");

// 创建示例数据
var customers = new List<Customer>
{
	new Customer { Id = 1, Name = "Alice", City = "Paris", Country = "France", Age = 45, Email = "alice@example.com" },
	new Customer { Id = 2, Name = "Bob", City = "London", Country = "UK", Age = 38, Email = "bob@example.com" },
	new Customer { Id = 3, Name = "Charlie", City = "Paris", Country = "France", Age = 55, Email = "charlie@example.com" },
	new Customer { Id = 4, Name = "Diana", City = "Berlin", Country = "Germany", Age = 42, Email = "diana@example.com" },
	new Customer { Id = 5, Name = "Eve", City = "Paris", Country = "France", Age = 28, Email = "eve@example.com" },
	new Customer { Id = 6, Name = "Frank", City = "London", Country = "UK", Age = 65, Email = "frank@example.com" }
};

var query = customers.AsQueryable();

// ============================================
// 示例1: 选择所有属性 (默认行为)
// ============================================
Console.WriteLine("【示例1】选择所有属性");
Console.WriteLine(new string('-', 100));

// 强类型 LINQ
var stronglyTyped1 = query
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers.ToList();");
Console.WriteLine($"  结果数量: {stronglyTyped1.Count}");
Console.WriteLine("  样本数据:");
foreach (var customer in stronglyTyped1.Take(2))
{
	Console.WriteLine($"    - ID: {customer.Id}, Name: {customer.Name}, City: {customer.City}, Age: {customer.Age}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例2: 选择特定属性到匿名类型
// ============================================
Console.WriteLine("【示例2】选择特定属性 (投影到匿名类型)");
Console.WriteLine(new string('-', 100));

// 强类型 LINQ
var stronglyTyped2 = query
	.Select(c => new { c.City, c.Name })
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Select(c => new { c.City, c.Name })");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {stronglyTyped2.Count}");
Console.WriteLine("  样本数据:");
foreach (var item in stronglyTyped2.Take(3))
{
	Console.WriteLine($"    - Name: {item.Name}, City: {item.City}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq1 = query
	.Select("new { City, Name }")
	.ToDynamicList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Select(\"new { City, Name }\")");
Console.WriteLine("    .ToDynamicList();");
Console.WriteLine($"  结果数量: {dynamicLinq1.Count}");
Console.WriteLine("  样本数据:");
foreach (var item in dynamicLinq1.Take(3))
{
	Console.WriteLine($"    - Name: {item.Name}, City: {item.City}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例3: 选择单个属性
// ============================================
Console.WriteLine("【示例3】选择单个属性");
Console.WriteLine(new string('-', 100));

// 强类型 LINQ
var stronglyTyped3 = query
	.Select(c => c.Name)
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Select(c => c.Name)");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {stronglyTyped3.Count}");
Console.WriteLine("  样本数据:");
foreach (var name in stronglyTyped3.Take(5))
{
	Console.WriteLine($"    - {name}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq2 = query
	.Select("Name")
	.ToDynamicList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Select(\"Name\")");
Console.WriteLine("    .ToDynamicList();");
Console.WriteLine($"  结果数量: {dynamicLinq2.Count}");
Console.WriteLine("  样本数据:");
foreach (var name in dynamicLinq2.Take(5))
{
	Console.WriteLine($"    - {name}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例4: 带有计算字段的投影
// ============================================
Console.WriteLine("【示例4】带有计算字段的投影");
Console.WriteLine(new string('-', 100));

// 强类型 LINQ
var stronglyTyped4 = query
	.Select(c => new 
	{ 
		c.Name, 
		c.City,
		FullInfo = c.Name + " from " + c.City,
		IsAdult = c.Age >= 18
	})
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Select(c => new { c.Name, c.City,");
Console.WriteLine("      FullInfo = c.Name + \" from \" + c.City,");
Console.WriteLine("      IsAdult = c.Age >= 18 })");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {stronglyTyped4.Count}");
Console.WriteLine("  样本数据:");
foreach (var item in stronglyTyped4.Take(3))
{
	Console.WriteLine($"    - {item.FullInfo}, Adult: {item.IsAdult}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq3 = query
	.Select("new { Name, City, FullInfo = Name + \" from \" + City, IsAdult = Age >= 18 }")
	.ToDynamicList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Select(\"new { Name, City,");
Console.WriteLine("      FullInfo = Name + \\\" from \\\" + City,");
Console.WriteLine("      IsAdult = Age >= 18 }\")");
Console.WriteLine("    .ToDynamicList();");
Console.WriteLine($"  结果数量: {dynamicLinq3.Count}");
Console.WriteLine("  样本数据:");
foreach (var item in dynamicLinq3.Take(3))
{
	Console.WriteLine($"    - {item.FullInfo}, Adult: {item.IsAdult}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例5: Select + Where 组合查询
// ============================================
Console.WriteLine("【示例5】Select + Where 组合查询");
Console.WriteLine(new string('-', 100));

// 强类型 LINQ
var stronglyTyped5 = query
	.Where(c => c.Age > 40)
	.Select(c => new { c.Name, c.City, c.Age })
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Where(c => c.Age > 40)");
Console.WriteLine("    .Select(c => new { c.Name, c.City, c.Age })");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {stronglyTyped5.Count}");
Console.WriteLine("  样本数据:");
foreach (var item in stronglyTyped5)
{
	Console.WriteLine($"    - {item.Name}, {item.City}, Age: {item.Age}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq4 = query
	.Where("Age > @0", 40)
	.Select("new { Name, City, Age }")
	.ToDynamicList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Where(\"Age > @0\", 40)");
Console.WriteLine("    .Select(\"new { Name, City, Age }\")");
Console.WriteLine("    .ToDynamicList();");
Console.WriteLine($"  结果数量: {dynamicLinq4.Count}");
Console.WriteLine("  样本数据:");
foreach (var item in dynamicLinq4)
{
	Console.WriteLine($"    - {item.Name}, {item.City}, Age: {item.Age}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例6: 带索引的 Select (Select with Index)
// ============================================
Console.WriteLine("【示例6】带索引的 Select");
Console.WriteLine(new string('-', 100));

// 动态 LINQ - 使用索引
var dynamicLinq5 = query
	.Select("new(it as Customer, $index as Index)")
	.ToDynamicList();

Console.WriteLine("动态 LINQ (带索引):");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Select(\"new(it as Customer, $index as Index)\")");
Console.WriteLine("    .ToDynamicList();");
Console.WriteLine($"  结果数量: {dynamicLinq5.Count}");
Console.WriteLine("  样本数据 (前3条):");
var i = 0;
foreach (var item in dynamicLinq5.Take(3))
{
	Console.WriteLine($"    Index: {item.Index}, Customer: {item.Customer.Name}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例7: 使用 ToString 方法投影
// ============================================
Console.WriteLine("【示例7】使用方法调用投影");
Console.WriteLine(new string('-', 100));

// 动态 LINQ
var dynamicLinq6 = query
	.Select("new { Name, NameLength = Name.Length, CityUpper = City.ToUpper() }")
	.ToDynamicList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Select(\"new { Name, NameLength = Name.Length,");
Console.WriteLine("      CityUpper = City.ToUpper() }\")");
Console.WriteLine("    .ToDynamicList();");
Console.WriteLine($"  结果数量: {dynamicLinq6.Count}");
Console.WriteLine("  样本数据:");
foreach (var item in dynamicLinq6.Take(3))
{
	Console.WriteLine($"    - {item.Name} (长度: {item.NameLength}), City: {item.CityUpper}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 总结
// ============================================
Console.WriteLine("【总结】");
Console.WriteLine(new string('-', 100));
Console.WriteLine("Select 子句的特点:");
Console.WriteLine("  1. 支持投影到匿名类型");
Console.WriteLine("  2. 支持选择单个属性");
Console.WriteLine("  3. 支持计算字段");
Console.WriteLine("  4. 支持方法调用");
Console.WriteLine("  5. 必须使用 ToDynamicList() 将动态结果转换为列表");
Console.WriteLine();
Console.WriteLine("性能优化:");
Console.WriteLine("  - Select 应该用于只需要部分字段的场景");
Console.WriteLine("  - 这样可以减少数据库返回的数据量");
Console.WriteLine("  - 特别是在处理大量数据时效果显著");
Console.WriteLine();

public class Customer
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string City { get; set; }
	public string Country { get; set; }
	public int Age { get; set; }
	public string Email { get; set; }
}
