<Query Kind="Statements">
  <NuGetReference>System.Linq.Dynamic.Core</NuGetReference>
  <Namespace>System.Linq.Dynamic.Core</Namespace>
</Query>

// ============================================
// Dynamic LINQ - Simple Query - 综合示例
// ============================================
// 此示例演示了 Where、Select、OrderBy 的综合使用
// 以及更复杂的查询场景

Console.WriteLine("=== Dynamic LINQ 综合查询示例 ===\n");

// 创建示例数据
var orders = new List<Order>
{
	new Order { Id = 1, CustomerId = 1, OrderDate = new DateTime(2024, 1, 15), Amount = 1500m, Status = "Completed" },
	new Order { Id = 2, CustomerId = 2, OrderDate = new DateTime(2024, 1, 20), Amount = 2300m, Status = "Pending" },
	new Order { Id = 3, CustomerId = 1, OrderDate = new DateTime(2024, 2, 10), Amount = 800m, Status = "Completed" },
	new Order { Id = 4, CustomerId = 3, OrderDate = new DateTime(2024, 2, 15), Amount = 5600m, Status = "Completed" },
	new Order { Id = 5, CustomerId = 2, OrderDate = new DateTime(2024, 2, 20), Amount = 1200m, Status = "Cancelled" },
	new Order { Id = 6, CustomerId = 1, OrderDate = new DateTime(2024, 3, 05), Amount = 3400m, Status = "Pending" },
	new Order { Id = 7, CustomerId = 3, OrderDate = new DateTime(2024, 3, 10), Amount = 2100m, Status = "Completed" },
	new Order { Id = 8, CustomerId = 4, OrderDate = new DateTime(2024, 3, 12), Amount = 950m, Status = "Completed" }
};

var query = orders.AsQueryable();

// ============================================
// 示例1: 完整的查询流程
// ============================================
Console.WriteLine("【示例1】完整查询流程 (Where + OrderBy + Select)");
Console.WriteLine(new string('-', 120));

// 强类型 LINQ
var stronglyTyped1 = query
	.Where(o => o.Status == "Completed" && o.Amount > 1000m)
	.OrderBy(o => o.OrderDate)
	.ThenByDescending(o => o.Amount)
	.Select(o => new { o.Id, o.CustomerId, o.OrderDate, o.Amount })
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = orders");
Console.WriteLine("    .Where(o => o.Status == \"Completed\" && o.Amount > 1000m)");
Console.WriteLine("    .OrderBy(o => o.OrderDate)");
Console.WriteLine("    .ThenByDescending(o => o.Amount)");
Console.WriteLine("    .Select(o => new { o.Id, o.CustomerId, o.OrderDate, o.Amount })");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {stronglyTyped1.Count}");
foreach (var item in stronglyTyped1)
{
	Console.WriteLine($"    - Order #{item.Id} | Date: {item.OrderDate:yyyy-MM-dd} | Amount: ${item.Amount}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq1 = query
	.Where("Status == @0 and Amount > @1", "Completed", 1000m)
	.OrderBy("OrderDate, Amount desc")
	.Select("new { Id, CustomerId, OrderDate, Amount }")
	.ToDynamicList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = orders");
Console.WriteLine("    .Where(\"Status == @0 and Amount > @1\", \"Completed\", 1000m)");
Console.WriteLine("    .OrderBy(\"OrderDate, Amount desc\")");
Console.WriteLine("    .Select(\"new { Id, CustomerId, OrderDate, Amount }\")");
Console.WriteLine("    .ToDynamicList();");
Console.WriteLine($"  结果数量: {dynamicLinq1.Count}");
foreach (var item in dynamicLinq1)
{
	Console.WriteLine($"    - Order #{item.Id} | Date: {item.OrderDate:yyyy-MM-dd} | Amount: ${item.Amount}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例2: 按条件分组统计
// ============================================
Console.WriteLine("【示例2】按状态统计订单");
Console.WriteLine(new string('-', 120));

// 强类型 LINQ
var stronglyTyped2 = query
	.GroupBy(o => o.Status)
	.Select(g => new 
	{ 
		Status = g.Key,
		Count = g.Count(),
		TotalAmount = g.Sum(o => o.Amount),
		AvgAmount = g.Average(o => o.Amount)
	})
	.OrderByDescending(x => x.Count)
	.ToList();

Console.WriteLine("强类型 LINQ (GroupBy):");
Console.WriteLine("  var result = orders");
Console.WriteLine("    .GroupBy(o => o.Status)");
Console.WriteLine("    .Select(g => new { Status = g.Key, Count = g.Count(), ... })");
Console.WriteLine("    .OrderByDescending(x => x.Count)");
Console.WriteLine("    .ToList();");
Console.WriteLine();
foreach (var item in stronglyTyped2)
{
	Console.WriteLine($"    - {item.Status,-10} | 数量: {item.Count} | 总额: ${item.TotalAmount} | 平均: ${item.AvgAmount:F2}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例3: 复杂的 Where 条件
// ============================================
Console.WriteLine("【示例3】复杂条件查询");
Console.WriteLine(new string('-', 120));

var targetDate = new DateTime(2024, 2, 1);
var minAmount = 1000m;
var maxAmount = 4000m;

// 强类型 LINQ
var stronglyTyped3 = query
	.Where(o => (o.Status == "Completed" || o.Status == "Pending") &&
			   o.OrderDate >= targetDate &&
			   o.Amount >= minAmount &&
			   o.Amount <= maxAmount)
	.OrderByDescending(o => o.Amount)
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = orders");
Console.WriteLine("    .Where(o => (o.Status == \"Completed\" || o.Status == \"Pending\") &&");
Console.WriteLine("               o.OrderDate >= targetDate &&");
Console.WriteLine("               o.Amount >= minAmount &&");
Console.WriteLine("               o.Amount <= maxAmount)");
Console.WriteLine("    .OrderByDescending(o => o.Amount)");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {stronglyTyped3.Count}");
foreach (var item in stronglyTyped3)
{
	Console.WriteLine($"    - Order #{item.Id} | {item.Status,-10} | Date: {item.OrderDate:yyyy-MM-dd} | ${item.Amount}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq3 = query
	.Where("(Status == @0 or Status == @1) and OrderDate >= @2 and Amount >= @3 and Amount <= @4",
		"Completed", "Pending", targetDate, minAmount, maxAmount)
	.OrderBy("Amount desc")
	.ToList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = orders");
Console.WriteLine("    .Where(\"(Status == @0 or Status == @1) and OrderDate >= @2 and Amount >= @3 and Amount <= @4\",");
Console.WriteLine("      \"Completed\", \"Pending\", targetDate, minAmount, maxAmount)");
Console.WriteLine("    .OrderBy(\"Amount desc\")");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {dynamicLinq3.Count}");
foreach (var item in dynamicLinq3)
{
	Console.WriteLine($"    - Order #{item.Id} | {item.Status,-10} | Date: {item.OrderDate:yyyy-MM-dd} | ${item.Amount}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例4: 带有计算列的投影
// ============================================
Console.WriteLine("【示例4】带有计算列的投影");
Console.WriteLine(new string('-', 120));

// 强类型 LINQ
var stronglyTyped4 = query
	.Where(o => o.Status == "Completed")
	.Select(o => new
	{
		o.Id,
		o.Amount,
		Tax = o.Amount * 0.1m,
		NetAmount = o.Amount * 0.9m,
		FormattedDate = o.OrderDate.ToString("yyyy-MM-dd"),
		Quarter = "Q" + ((o.OrderDate.Month - 1) / 3 + 1)
	})
	.OrderBy(x => x.Quarter)
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = orders");
Console.WriteLine("    .Where(o => o.Status == \"Completed\")");
Console.WriteLine("    .Select(o => new { o.Id, Tax = o.Amount * 0.1m, ... })");
Console.WriteLine("    .OrderBy(x => x.Quarter)");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  结果数量: {stronglyTyped4.Count}");
foreach (var item in stronglyTyped4)
{
	Console.WriteLine($"    - Order #{item.Id} | Amount: ${item.Amount} | Tax: ${item.Tax:F2} | Net: ${item.NetAmount:F2} | {item.Quarter}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq4 = query
	.Where("Status == @0", "Completed")
	.Select("new { Id, Amount, Tax = Amount * 0.1, NetAmount = Amount * 0.9 }")
	.ToDynamicList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = orders");
Console.WriteLine("    .Where(\"Status == @0\", \"Completed\")");
Console.WriteLine("    .Select(\"new { Id, Amount, Tax = Amount * 0.1, NetAmount = Amount * 0.9 }\")");
Console.WriteLine("    .ToDynamicList();");
Console.WriteLine($"  结果数量: {dynamicLinq4.Count}");
foreach (var item in dynamicLinq4)
{
	Console.WriteLine($"    - Order #{item.Id} | Amount: ${item.Amount} | Tax: ${item.Tax:F2} | Net: ${item.NetAmount:F2}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例5: 分页查询
// ============================================
Console.WriteLine("【示例5】分页查询");
Console.WriteLine(new string('-', 120));

int pageSize = 3;
int pageNumber = 1;

// 强类型 LINQ
var stronglyTyped5 = query
	.Where(o => o.Status == "Completed")
	.OrderByDescending(o => o.OrderDate)
	.Skip((pageNumber - 1) * pageSize)
	.Take(pageSize)
	.Select(o => new { o.Id, o.OrderDate, o.Amount })
	.ToList();

Console.WriteLine("强类型 LINQ (第1页，每页3条):");
Console.WriteLine("  var result = orders");
Console.WriteLine("    .Where(o => o.Status == \"Completed\")");
Console.WriteLine("    .OrderByDescending(o => o.OrderDate)");
Console.WriteLine("    .Skip(0)");
Console.WriteLine("    .Take(3)");
Console.WriteLine("    .ToList();");
foreach (var item in stronglyTyped5)
{
	Console.WriteLine($"    - Order #{item.Id} | Date: {item.OrderDate:yyyy-MM-dd} | ${item.Amount}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq5 = query
	.Where("Status == @0", "Completed")
	.OrderBy("OrderDate desc")
	.Skip((pageNumber - 1) * pageSize)
	.Take(pageSize)
	.Select("new { Id, OrderDate, Amount }")
	.ToDynamicList();

Console.WriteLine("动态 LINQ (第1页，每页3条):");
Console.WriteLine("  var result = orders");
Console.WriteLine("    .Where(\"Status == @0\", \"Completed\")");
Console.WriteLine("    .OrderBy(\"OrderDate desc\")");
Console.WriteLine("    .Skip(0)");
Console.WriteLine("    .Take(3)");
Console.WriteLine("    .Select(\"new { Id, OrderDate, Amount }\")");
Console.WriteLine("    .ToDynamicList();");
foreach (var item in dynamicLinq5)
{
	Console.WriteLine($"    - Order #{item.Id} | Date: {item.OrderDate:yyyy-MM-dd} | ${item.Amount}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 示例6: 去重查询
// ============================================
Console.WriteLine("【示例6】去重查询");
Console.WriteLine(new string('-', 120));

// 强类型 LINQ
var stronglyTyped6 = query
	.Where(o => o.Status == "Completed")
	.Select(o => o.CustomerId)
	.Distinct()
	.OrderBy(x => x)
	.ToList();

Console.WriteLine("强类型 LINQ:");
Console.WriteLine("  var result = orders");
Console.WriteLine("    .Where(o => o.Status == \"Completed\")");
Console.WriteLine("    .Select(o => o.CustomerId)");
Console.WriteLine("    .Distinct()");
Console.WriteLine("    .OrderBy(x => x)");
Console.WriteLine("    .ToList();");
Console.WriteLine($"  不同客户数量: {stronglyTyped6.Count}");
foreach (var customerId in stronglyTyped6)
{
	Console.WriteLine($"    - Customer #{customerId}");
}

Console.WriteLine();

// 动态 LINQ
var dynamicLinq6 = query
	.Where("Status == @0", "Completed")
	.Select("CustomerId")
	.Distinct()
	.OrderBy("@0")
	.ToDynamicList();

Console.WriteLine("动态 LINQ:");
Console.WriteLine("  var result = orders");
Console.WriteLine("    .Where(\"Status == @0\", \"Completed\")");
Console.WriteLine("    .Select(\"CustomerId\")");
Console.WriteLine("    .Distinct()");
Console.WriteLine("    .OrderBy(\"@0\")");
Console.WriteLine("    .ToDynamicList();");
Console.WriteLine($"  不同客户数量: {dynamicLinq6.Count}");
foreach (var customerId in dynamicLinq6)
{
	Console.WriteLine($"    - Customer #{customerId}");
}

Console.WriteLine();
Console.WriteLine();

// ============================================
// 总结
// ============================================
Console.WriteLine("【总结】");
Console.WriteLine(new string('-', 120));
Console.WriteLine("Dynamic LINQ 查询执行顺序建议:");
Console.WriteLine("  1. Where     - 首先筛选数据（最重要的优化点）");
Console.WriteLine("  2. OrderBy   - 然后排序");
Console.WriteLine("  3. Skip/Take - 分页（如需要）");
Console.WriteLine("  4. Select    - 最后投影（可以减少传输的数据量）");
Console.WriteLine();
Console.WriteLine("常见错误:");
Console.WriteLine("  ✗ .Select(...).Where(...) - 应该先Where后Select");
Console.WriteLine("  ✗ .Where(...).Select(...).OrderBy(...) - OrderBy应在Select之前");
Console.WriteLine("  ✓ .Where(...).OrderBy(...).Skip(...).Take(...).Select(...) - 正确顺序");
Console.WriteLine();
Console.WriteLine("性能建议:");
Console.WriteLine("  - 尽可能在数据库端完成过滤和排序");
Console.WriteLine("  - 只选择必要的列来减少网络传输");
Console.WriteLine("  - 合理使用参数化查询防止 SQL 注入");
Console.WriteLine("  - 避免在客户端进行大量数据处理");
Console.WriteLine();

public class Order
{
	public int Id { get; set; }
	public int CustomerId { get; set; }
	public DateTime OrderDate { get; set; }
	public decimal Amount { get; set; }
	public string Status { get; set; } // Pending, Completed, Cancelled
}
