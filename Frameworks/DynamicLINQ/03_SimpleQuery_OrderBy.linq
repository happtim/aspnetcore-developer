<Query Kind="Statements">
  <NuGetReference>System.Linq.Dynamic.Core</NuGetReference>
  <Namespace>System.Linq.Dynamic.Core</Namespace>
</Query>

// ============================================
// Dynamic LINQ - Simple Query - ORDERBY 示例
// ============================================
// 此示例演示了 OrderBy 子句的各种用法
// 包括：单字段排序、多字段排序、升序/降序、混合排序

Console.WriteLine("=== Dynamic LINQ OrderBy 查询示例 ===\n");

// 创建示例数据
var customers = new List<Customer>
{
	new Customer { Id = 1, Name = "Alice", City = "Paris", Country = "France", Age = 45, Salary = 75000m },
	new Customer { Id = 2, Name = "Bob", City = "London", Country = "UK", Age = 38, Salary = 68000m },
	new Customer { Id = 3, Name = "Charlie", City = "Paris", Country = "France", Age = 55, Salary = 95000m },
	new Customer { Id = 4, Name = "Diana", City = "Berlin", Country = "Germany", Age = 42, Salary = 72000m },
	new Customer { Id = 5, Name = "Eve", City = "Paris", Country = "France", Age = 28, Salary = 55000m },
	new Customer { Id = 6, Name = "Frank", City = "London", Country = "UK", Age = 65, Salary = 120000m },
	new Customer { Id = 7, Name = "Grace", City = "Berlin", Country = "Germany", Age = 35, Salary = 62000m },
	new Customer { Id = 8, Name = "Henry", City = "Paris", Country = "France", Age = 52, Salary = 88000m }
};

var query = customers.AsQueryable();

// ============================================
// 示例1: 单字段升序排序
// ============================================
Console.WriteLine("【示例1】单字段升序排序");
Console.WriteLine(new string('-', 100));

// 强类型 LINQ
var stronglyTyped1 = query
	.OrderBy(c => c.Name)
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderBy(c => c.Name)");
Console.WriteLine("    .ToList();");
Console.WriteLine("  结果 (按名字升序):");
foreach (var customer in stronglyTyped1)
{
	Console.WriteLine($"    - {customer.Name}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq1 = query
	.OrderBy("Name")
	.ToList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderBy(\"Name\")");
Console.WriteLine("    .ToList();");
Console.WriteLine("  结果 (按名字升序):");
foreach (var customer in dynamicLinq1)
{
	Console.WriteLine($"    - {customer.Name}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例2: 单字段降序排序
// ============================================
Console.WriteLine("【示例2】单字段降序排序");
Console.WriteLine(new string('-', 100));

// 强类型 LINQ
var stronglyTyped2 = query
	.OrderByDescending(c => c.Age)
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderByDescending(c => c.Age)");
Console.WriteLine("    .ToList();");
Console.WriteLine("  结果 (按年龄降序):");
foreach (var customer in stronglyTyped2.Take(5))
{
	Console.WriteLine($"    - {customer.Name}, Age: {customer.Age}");
}

Console.WriteLine();

// 动态 LINQ - 方法1
var dynamicLinq2 = query
	.OrderBy("Age desc")
	.ToList();

Console.WriteLine("动态 LINQ (方法1 - desc 后缀):");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderBy(\"Age desc\")");
Console.WriteLine("    .ToList();");
Console.WriteLine("  结果 (按年龄降序):");
foreach (var customer in dynamicLinq2.Take(5))
{
	Console.WriteLine($"    - {customer.Name}, Age: {customer.Age}");
}

Console.WriteLine();

// 动态 LINQ - 方法2
var dynamicLinq3 = query
	.OrderByDescending("Age")
	.ToList();

Console.WriteLine("动态 LINQ (方法2 - OrderByDescending):");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderByDescending(\"Age\")");
Console.WriteLine("    .ToList();");
Console.WriteLine("  结果 (按年龄降序):");
foreach (var customer in dynamicLinq3.Take(5))
{
	Console.WriteLine($"    - {customer.Name}, Age: {customer.Age}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例3: 多字段排序 (升序)
// ============================================
Console.WriteLine("【示例3】多字段排序 (都是升序)");
Console.WriteLine(new string('-', 100));

// 强类型 LINQ
var stronglyTyped3 = query
	.OrderBy(c => c.City)
	.ThenBy(c => c.Name)
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderBy(c => c.City)");
Console.WriteLine("    .ThenBy(c => c.Name)");
Console.WriteLine("    .ToList();");
Console.WriteLine("  结果 (先按城市升序，再按名字升序):");
foreach (var customer in stronglyTyped3)
{
	Console.WriteLine($"    - {customer.City,-10} | {customer.Name}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq4 = query
	.OrderBy("City, Name")
	.ToList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderBy(\"City, Name\")");
Console.WriteLine("    .ToList();");
Console.WriteLine("  结果 (先按城市升序，再按名字升序):");
foreach (var customer in dynamicLinq4)
{
	Console.WriteLine($"    - {customer.City,-10} | {customer.Name}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例4: 多字段混合排序 (升序和降序)
// ============================================
Console.WriteLine("【示例4】多字段混合排序 (升序和降序)");
Console.WriteLine(new string('-', 100));

// 强类型 LINQ
var stronglyTyped4 = query
	.OrderBy(c => c.City)
	.ThenByDescending(c => c.Age)
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderBy(c => c.City)");
Console.WriteLine("    .ThenByDescending(c => c.Age)");
Console.WriteLine("    .ToList();");
Console.WriteLine("  结果 (按城市升序，再按年龄降序):");
foreach (var customer in stronglyTyped4)
{
	Console.WriteLine($"    - {customer.City,-10} | {customer.Name,-8} | Age: {customer.Age}");
}

Console.WriteLine();

// 动态 LINQ - 方法1
var dynamicLinq5 = query
	.OrderBy("City, Age desc")
	.ToList();

Console.WriteLine("动态 LINQ (方法1):");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderBy(\"City, Age desc\")");
Console.WriteLine("    .ToList();");
Console.WriteLine("  结果 (按城市升序，再按年龄降序):");
foreach (var customer in dynamicLinq5)
{
	Console.WriteLine($"    - {customer.City,-10} | {customer.Name,-8} | Age: {customer.Age}");
}

Console.WriteLine();

// 动态 LINQ - 方法2 (使用 ThenBy)
var dynamicLinq6 = query
	.OrderBy("City")
	.ThenBy("Age desc")
	.ToList();

Console.WriteLine("动态 LINQ (方法2 - 使用 ThenBy):");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderBy(\"City\")");
Console.WriteLine("    .ThenBy(\"Age desc\")");
Console.WriteLine("    .ToList();");
Console.WriteLine("  结果 (按城市升序，再按年龄降序):");
foreach (var customer in dynamicLinq6)
{
	Console.WriteLine($"    - {customer.City,-10} | {customer.Name,-8} | Age: {customer.Age}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例5: 复杂混合排序
// ============================================
Console.WriteLine("【示例5】复杂混合排序 (三个字段)");
Console.WriteLine(new string('-', 100));

// 强类型 LINQ
var stronglyTyped5 = query
	.OrderBy(c => c.City)
	.ThenByDescending(c => c.Salary)
	.ThenBy(c => c.Name)
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderBy(c => c.City)");
Console.WriteLine("    .ThenByDescending(c => c.Salary)");
Console.WriteLine("    .ThenBy(c => c.Name)");
Console.WriteLine("    .ToList();");
Console.WriteLine("  结果 (城市升序 > 薪资降序 > 名字升序):");
foreach (var customer in stronglyTyped5)
{
	Console.WriteLine($"    - {customer.City,-10} | Salary: ${customer.Salary,8} | {customer.Name}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq7 = query
	.OrderBy("City, Salary desc, Name")
	.ToList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderBy(\"City, Salary desc, Name\")");
Console.WriteLine("    .ToList();");
Console.WriteLine("  结果 (城市升序 > 薪资降序 > 名字升序):");
foreach (var customer in dynamicLinq7)
{
	Console.WriteLine($"    - {customer.City,-10} | Salary: ${customer.Salary,8} | {customer.Name}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例6: OrderBy + Where 组合
// ============================================
Console.WriteLine("【示例6】OrderBy + Where 组合查询");
Console.WriteLine(new string('-', 100));

// 强类型 LINQ
var stronglyTyped6 = query
	.Where(c => c.Salary > 70000m)
	.OrderBy(c => c.City)
	.ThenByDescending(c => c.Salary)
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Where(c => c.Salary > 70000m)");
Console.WriteLine("    .OrderBy(c => c.City)");
Console.WriteLine("    .ThenByDescending(c => c.Salary)");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {stronglyTyped6.Count}");
foreach (var customer in stronglyTyped6)
{
	Console.WriteLine($"    - {customer.City,-10} | {customer.Name,-8} | Salary: ${customer.Salary}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq8 = query
	.Where("Salary > @0", 70000m)
	.OrderBy("City, Salary desc")
	.ToList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .Where(\"Salary > @0\", 70000m)");
Console.WriteLine("    .OrderBy(\"City, Salary desc\")");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {dynamicLinq8.Count}");
foreach (var customer in dynamicLinq8)
{
	Console.WriteLine($"    - {customer.City,-10} | {customer.Name,-8} | Salary: ${customer.Salary}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例7: OrderBy + Select 组合
// ============================================
Console.WriteLine("【示例7】OrderBy + Select 组合查询");
Console.WriteLine(new string('-', 100));

// 强类型 LINQ
var stronglyTyped7 = query
	.OrderBy(c => c.City)
	.ThenBy(c => c.Name)
	.Select(c => new { c.Name, c.City, c.Salary })
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderBy(c => c.City)");
Console.WriteLine("    .ThenBy(c => c.Name)");
Console.WriteLine("    .Select(c => new { c.Name, c.City, c.Salary })");
Console.WriteLine("    .ToList();");
foreach (var customer in stronglyTyped7.Take(5))
{
	Console.WriteLine($"    - {customer.City,-10} | {customer.Name,-8} | ${customer.Salary}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq9 = query
	.OrderBy("City, Name")
	.Select("new { Name, City, Salary }")
	.ToDynamicList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = context.Customers");
Console.WriteLine("    .OrderBy(\"City, Name\")");
Console.WriteLine("    .Select(\"new { Name, City, Salary }\")");
Console.WriteLine("    .ToDynamicList();");
foreach (var customer in dynamicLinq9.Take(5))
{
	Console.WriteLine($"    - {customer.City,-10} | {customer.Name,-8} | ${customer.Salary}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 总结
// ============================================
Console.WriteLine("【总结】");
Console.WriteLine(new string('-', 100));
Console.WriteLine("OrderBy 排序特性:");
Console.WriteLine("  1. 单字段排序: OrderBy(\"FieldName\")");
Console.WriteLine("  2. 多字段排序: OrderBy(\"Field1, Field2, Field3\")");
Console.WriteLine("  3. 升序: 默认或使用 ascending 后缀");
Console.WriteLine("  4. 降序: 使用 desc 或 descending 后缀");
Console.WriteLine("  5. 混合排序: OrderBy(\"Field1, Field2 desc, Field3\")");
Console.WriteLine();
Console.WriteLine("重要说明:");
Console.WriteLine("  - 不能在字段名上使用 @0 这样的参数占位符");
Console.WriteLine("  - 排序字段名必须硬编码在字符串中");
Console.WriteLine("  - 排序方向也必须硬编码");
Console.WriteLine("  - 只有常量值可以使用参数占位符 @0, @1 等");
Console.WriteLine();
Console.WriteLine("最佳实践:");
Console.WriteLine("  - 将 Where 放在 OrderBy 之前以提高性能");
Console.WriteLine("  - 将 OrderBy 放在 Select 之前");
Console.WriteLine("  - 避免对大型结果集进行客户端排序");
Console.WriteLine();

public class Customer
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string City { get; set; }
	public string Country { get; set; }
	public int Age { get; set; }
	public decimal Salary { get; set; }
}
