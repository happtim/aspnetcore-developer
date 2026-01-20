<Query Kind="Statements">
  <NuGetReference>System.Linq.Dynamic.Core</NuGetReference>
  <Namespace>System.Linq.Dynamic.Core</Namespace>
</Query>

// ============================================
// Dynamic LINQ - Simple Query - WHERE 示例
// ============================================
// 此示例演示了 Where 子句的各种用法
// 包括：单条件、多条件、参数化查询

Console.WriteLine("=== Dynamic LINQ Where 查询示例 ===\n");

// 创建示例数据
var customers = new List<Customer>
{
	new Customer { Id = 1, Name = "Alice", City = "Paris", Age = 45 },
	new Customer { Id = 2, Name = "Bob", City = "London", Age = 38 },
	new Customer { Id = 3, Name = "Charlie", City = "Paris", Age = 55 },
	new Customer { Id = 4, Name = "Diana", City = "Berlin", Age = 42 },
	new Customer { Id = 5, Name = "Eve", City = "Paris", Age = 28 },
	new Customer { Id = 6, Name = "Frank", City = "London", Age = 65 }
};

var query = customers.AsQueryable();

// ============================================
// 示例1: 单条件查询 - 强类型 vs 动态 LINQ
// ============================================
Console.WriteLine("【示例1】单条件查询");
Console.WriteLine(new string('-', 80));

// 强类型 LINQ
var stronglyTyped = query
	.Where(c => c.City == "Paris")
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers.Where(c => c.City == \"Paris\").ToList();");
Console.WriteLine($"  结果数量: {stronglyTyped.Count}");
foreach (var customer in stronglyTyped)
{
	Console.WriteLine($"    - {customer.Name}, {customer.City}, Age: {customer.Age}");
}

Console.WriteLine();

// 动态 LINQ - 使用字符串拼接
var dynamicLinq1 = query
	.Where("City == \"Paris\"")
	.ToList();

Console.WriteLine("动态 LINQ (字符串拼接):");
Console.WriteLine("  var result = context.Customers.Where(\"City == \\\"Paris\\\"\").ToList();");
Console.WriteLine($"  结果数量: {dynamicLinq1.Count}");
foreach (var customer in dynamicLinq1)
{
	Console.WriteLine($"    - {customer.Name}, {customer.City}, Age: {customer.Age}");
}

Console.WriteLine();

// 动态 LINQ - 使用参数化查询
var dynamicLinq2 = query
	.Where("City == @0", "Paris")
	.ToList();

Console.WriteLine("动态 LINQ (参数化查询):");
Console.WriteLine("  var result = context.Customers.Where(\"City == @0\", \"Paris\").ToList();");
Console.WriteLine($"  结果数量: {dynamicLinq2.Count}");
foreach (var customer in dynamicLinq2)
{
	Console.WriteLine($"    - {customer.Name}, {customer.City}, Age: {customer.Age}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例2: 多条件查询 (AND 操作)
// ============================================
Console.WriteLine("【示例2】多条件查询 (AND)");
Console.WriteLine(new string('-', 80));

// 强类型 LINQ
var stronglyTyped2 = query
	.Where(c => c.City == "Paris" && c.Age > 50)
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Where(c => c.City == \"Paris\" && c.Age > 50)");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {stronglyTyped2.Count}");
foreach (var customer in stronglyTyped2)
{
	Console.WriteLine($"    - {customer.Name}, {customer.City}, Age: {customer.Age}");
}

Console.WriteLine();

// 动态 LINQ - 使用字符串拼接
var dynamicLinq3 = query
	.Where("City == \"Paris\" && Age > 50")
	.ToList();

Console.WriteLine("动态 LINQ (字符串拼接):");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Where(\"City == \\\"Paris\\\" && Age > 50\")");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {dynamicLinq3.Count}");
foreach (var customer in dynamicLinq3)
{
	Console.WriteLine($"    - {customer.Name}, {customer.City}, Age: {customer.Age}");
}

Console.WriteLine();

// 动态 LINQ - 使用参数化查询
var dynamicLinq4 = query
	.Where("City == @0 and Age > @1", "Paris", 50)
	.ToList();

Console.WriteLine("动态 LINQ (参数化查询):");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Where(\"City == @0 and Age > @1\", \"Paris\", 50)");
Console.WriteLine("    .ToList();");
Console.WriteLine("  注意：可以使用 and 代替 &&");
Console.WriteLine($"  结果数量: {dynamicLinq4.Count}");
foreach (var customer in dynamicLinq4)
{
	Console.WriteLine($"    - {customer.Name}, {customer.City}, Age: {customer.Age}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例3: 多条件查询 (OR 操作)
// ============================================
Console.WriteLine("【示例3】多条件查询 (OR)");
Console.WriteLine(new string('-', 80));

// 强类型 LINQ
var stronglyTyped3 = query
	.Where(c => c.City == "Paris" || c.City == "London")
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Where(c => c.City == \"Paris\" || c.City == \"London\")");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {stronglyTyped3.Count}");
foreach (var customer in stronglyTyped3)
{
	Console.WriteLine($"    - {customer.Name}, {customer.City}");
}

Console.WriteLine();

// 动态 LINQ - 使用参数化查询
var dynamicLinq5 = query
	.Where("City == @0 or City == @1", "Paris", "London")
	.ToList();

Console.WriteLine("动态 LINQ (参数化查询):");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Where(\"City == @0 or City == @1\", \"Paris\", \"London\")");
Console.WriteLine("    .ToList();");
Console.WriteLine("  注意：可以使用 or 代替 ||");
Console.WriteLine($"  结果数量: {dynamicLinq5.Count}");
foreach (var customer in dynamicLinq5)
{
	Console.WriteLine($"    - {customer.Name}, {customer.City}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例4: 使用 Lambda 表达式 (带参数前缀)
// ============================================
Console.WriteLine("【示例4】使用 Lambda 表达式");
Console.WriteLine(new string('-', 80));

var dynamicLinq6 = query
	.Where("c => c.City == \"Paris\" && c.Age > 50")
	.ToList();

Console.WriteLine("动态 LINQ (Lambda 表达式):");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Where(\"c => c.City == \\\"Paris\\\" && c.Age > 50\")");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {dynamicLinq6.Count}");
foreach (var customer in dynamicLinq6)
{
	Console.WriteLine($"    - {customer.Name}, {customer.City}, Age: {customer.Age}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例5: 字符串转义示例
// ============================================
Console.WriteLine("【示例5】字符串转义");
Console.WriteLine(new string('-', 80));

// 创建包含特殊字符的数据
var productsWithSpecialChars = new List<Product>
{
	new Product { Id = 1, Name = "Product \"A\"", Description = "Test" },
	new Product { Id = 2, Name = "Product (B)", Description = "Line\\Break" }
}.AsQueryable();

var result = productsWithSpecialChars
	.Where("Name == \"Product \\\"A\\\"\"")
	.ToList();

Console.WriteLine("查询包含引号的字符串:");
Console.WriteLine("  .Where(\"Name == \\\"Product \\\\\\\"A\\\\\\\"\\\"\")");
Console.WriteLine($"  结果数量: {result.Count}");
foreach (var product in result)
{
	Console.WriteLine($"    - {product.Name}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 总结
// ============================================
Console.WriteLine("【总结】");
Console.WriteLine(new string('-', 80));
Console.WriteLine("Where 子句支持的操作符:");
Console.WriteLine("  比较操作符: ==, !=, <, >, <=, >=");
Console.WriteLine("  逻辑操作符: && (and), || (or), ! (not)");
Console.WriteLine();
Console.WriteLine("参数化查询的优点:");
Console.WriteLine("  1. 安全性更高，防止 SQL 注入");
Console.WriteLine("  2. 代码更易读");
Console.WriteLine("  3. 性能更好（查询计划缓存）");
Console.WriteLine();

public class Customer
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string City { get; set; }
	public int Age { get; set; }
}

public class Product
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
}
