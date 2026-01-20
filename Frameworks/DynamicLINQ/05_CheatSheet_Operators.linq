<Query Kind="Statements">
  <NuGetReference>System.Linq.Dynamic.Core</NuGetReference>
  <Namespace>System.Linq.Dynamic.Core</Namespace>
</Query>

// ============================================
// Dynamic LINQ - 常见操作符和方法速查表
// ============================================

Console.WriteLine("=== Dynamic LINQ 常见操作符和方法速查表 ===\n");

// ============================================
// 1. 比较操作符
// ============================================
Console.WriteLine("【1. 比较操作符】");
Console.WriteLine(new string('-', 120));
Console.WriteLine("等于           ==        x.Age == 30");
Console.WriteLine("不等于         !=        x.Status != \"Completed\"");
Console.WriteLine("大于           >         x.Amount > 1000");
Console.WriteLine("小于           <         x.Amount < 5000");
Console.WriteLine("大于等于       >=        x.Age >= 18");
Console.WriteLine("小于等于       <=        x.Age <= 65");
Console.WriteLine();

// ============================================
// 2. 逻辑操作符
// ============================================
Console.WriteLine("【2. 逻辑操作符】");
Console.WriteLine(new string('-', 120));
Console.WriteLine("且 (AND)       &&  或 and         x.Age > 18 && x.Status == \"Active\"");
Console.WriteLine("或 (OR)        ||  或 or          x.City == \"Paris\" || x.City == \"London\"");
Console.WriteLine("非 (NOT)       !   或 not         !(x.Status == \"Cancelled\")");
Console.WriteLine();

// ============================================
// 3. 字符串方法
// ============================================
Console.WriteLine("【3. 字符串方法】");
Console.WriteLine(new string('-', 120));
var samples = new List<StringExample> { 
	new StringExample { Text = "Hello World" }, 
	new StringExample { Text = "LINQ Dynamic" } 
}.AsQueryable();

Console.WriteLine("  Contains       x.Name.Contains(\"ohn\")");
var result1 = samples.Where("Text.Contains(\"World\")").ToList();
Console.WriteLine($"    结果: {string.Join(", ", result1.Select(x => x.Text))}");

Console.WriteLine("  StartsWith     x.Name.StartsWith(\"J\")");
var result2 = samples.Where("Text.StartsWith(\"LINQ\")").ToList();
Console.WriteLine($"    结果: {string.Join(", ", result2.Select(x => x.Text))}");

Console.WriteLine("  EndsWith       x.Name.EndsWith(\"n\")");
var result3 = samples.Where("Text.EndsWith(\"World\")").ToList();
Console.WriteLine($"    结果: {string.Join(", ", result3.Select(x => x.Text))}");

Console.WriteLine("  ToUpper        x.Name.ToUpper()");
var result4 = samples.Select("Text.ToUpper()").ToDynamicList();
Console.WriteLine($"    结果: {string.Join(", ", result4)}");

Console.WriteLine("  ToLower        x.Name.ToLower()");
var result5 = samples.Select("Text.ToLower()").ToDynamicList();
Console.WriteLine($"    结果: {string.Join(", ", result5)}");

Console.WriteLine("  Length         x.Name.Length");
var result6 = samples.Select("Text.Length").ToDynamicList();
Console.WriteLine($"    结果: {string.Join(", ", result6)}");

Console.WriteLine("  Substring      x.Name.Substring(0, 5)");
var result7 = samples.Select("Text.Substring(0, 5)").ToDynamicList();
Console.WriteLine($"    结果: {string.Join(", ", result7)}");

Console.WriteLine("  Replace        x.Name.Replace(\"o\", \"0\")");
var result8 = samples.Select("Text.Replace(\"o\", \"0\")").ToDynamicList();
Console.WriteLine($"    结果: {string.Join(", ", result8)}");

Console.WriteLine("  Trim           x.Name.Trim()");
Console.WriteLine("  TrimStart      x.Name.TrimStart()");
Console.WriteLine("  TrimEnd        x.Name.TrimEnd()");
Console.WriteLine();

// ============================================
// 4. 数学操作符
// ============================================
Console.WriteLine("【4. 数学操作符】");
Console.WriteLine(new string('-', 120));
var numbers = new List<NumberExample> {
	new NumberExample { Value = 10 },
	new NumberExample { Value = 20 },
	new NumberExample { Value = 30 }
}.AsQueryable();

Console.WriteLine("  加法           +          x.Price + 10");
var result9 = numbers.Select("Value + 5").ToDynamicList();
Console.WriteLine($"    结果: {string.Join(", ", result9)}");

Console.WriteLine("  减法           -          x.Price - 10");
var result10 = numbers.Select("Value - 5").ToDynamicList();
Console.WriteLine($"    结果: {string.Join(", ", result10)}");

Console.WriteLine("  乘法           *          x.Price * 2");
var result11 = numbers.Select("Value * 2").ToDynamicList();
Console.WriteLine($"    结果: {string.Join(", ", result11)}");

Console.WriteLine("  除法           /          x.Price / 2");
var result12 = numbers.Select("Value / 2").ToDynamicList();
Console.WriteLine($"    结果: {string.Join(", ", result12)}");

Console.WriteLine("  取余           %          x.Price % 3");
var result13 = numbers.Select("Value % 3").ToDynamicList();
Console.WriteLine($"    结果: {string.Join(", ", result13)}");

Console.WriteLine();

// ============================================
// 5. 聚合方法
// ============================================
Console.WriteLine("【5. 聚合方法】");
Console.WriteLine(new string('-', 120));
var items = new List<Item> {
	new Item { Name = "Item1", Qty = 10, Price = 100m },
	new Item { Name = "Item2", Qty = 20, Price = 50m },
	new Item { Name = "Item3", Qty = 15, Price = 75m }
}.AsQueryable();

Console.WriteLine("Count()");
var count = items.Count();
Console.WriteLine($"    结果: {count}");

Console.WriteLine("Sum(x => x.Price)");
var sum = items.Select("Price").Cast<decimal>().Sum();
Console.WriteLine($"    结果: {sum}");

Console.WriteLine("Average(x => x.Price)");
var avg = items.Select("Price").Cast<decimal>().Average();
Console.WriteLine($"    结果: {avg:F2}");

Console.WriteLine("Min(x => x.Price)");
var min = items.Select("Price").Cast<decimal>().Min();
Console.WriteLine($"    结果: {min}");

Console.WriteLine("Max(x => x.Price)");
var max = items.Select("Price").Cast<decimal>().Max();
Console.WriteLine($"    结果: {max}");

Console.WriteLine();

// ============================================
// 6. 排序操作
// ============================================
Console.WriteLine("【6. 排序操作】");
Console.WriteLine(new string('-', 120));
Console.WriteLine("  升序排列       .OrderBy(\"FieldName\")");
Console.WriteLine("  降序排列       .OrderBy(\"FieldName desc\") 或 .OrderByDescending(\"FieldName\")");
Console.WriteLine("  多字段排序     .OrderBy(\"Field1, Field2 desc\")");
Console.WriteLine("  然后排序       .OrderBy(\"Field1\").ThenBy(\"Field2 desc\")");
Console.WriteLine("  反转序列       .Reverse()");
Console.WriteLine();

// ============================================
// 7. 投影操作
// ============================================
Console.WriteLine("【7. 投影操作】");
Console.WriteLine(new string('-', 120));
Console.WriteLine("  Select         .Select(\"Name\") 或 .Select(\"new { Name, Price }\")");
Console.WriteLine("  SelectMany     .SelectMany(\"Items\")");
Console.WriteLine("  Distinct       .Distinct()");
Console.WriteLine("  Skip           .Skip(10) - 跳过前10条");
Console.WriteLine("  Take           .Take(5)  - 取前5条");
Console.WriteLine("  First          .First() - 返回第一条");
Console.WriteLine("  FirstOrDefault .FirstOrDefault() - 返回第一条或默认值");
Console.WriteLine("  Last           .Last() - 返回最后一条");
Console.WriteLine("  LastOrDefault  .LastOrDefault() - 返回最后一条或默认值");
Console.WriteLine("  Single         .Single() - 返回唯一元素（没有或多于1个会异常）");
Console.WriteLine("  SingleOrDefault.SingleOrDefault() - 返回唯一元素或默认值");
Console.WriteLine();

// ============================================
// 8. 分组操作
// ============================================
Console.WriteLine("【8. 分组操作】");
Console.WriteLine(new string('-', 120));
Console.WriteLine("  GroupBy        .GroupBy(\"Category\")");
Console.WriteLine("  示例:");
var grouped = items.GroupBy(x => x.Name.Substring(0, 4)).Select(g => new { 
	Group = g.Key, 
	Count = g.Count() 
});
foreach (var g in grouped)
{
	Console.WriteLine($"    {g.Group}: {g.Count}");
}
Console.WriteLine();

// ============================================
// 9. 集合操作
// ============================================
Console.WriteLine("【9. 集合操作】");
Console.WriteLine(new string('-', 120));
var list1 = new List<int> { 1, 2, 3 };
var list2 = new List<int> { 3, 4, 5 };
Console.WriteLine("  Union          .Union(other) - 并集（去重）");
Console.WriteLine($"    {string.Join(", ", list1.Union(list2))}");
Console.WriteLine("  Intersect      .Intersect(other) - 交集");
Console.WriteLine($"    {string.Join(", ", list1.Intersect(list2))}");
Console.WriteLine("  Except         .Except(other) - 差集");
Console.WriteLine($"    {string.Join(", ", list1.Except(list2))}");
Console.WriteLine("  Concat         .Concat(other) - 连接（不去重）");
Console.WriteLine($"    {string.Join(", ", list1.Concat(list2))}");
Console.WriteLine();

// ============================================
// 10. 类型转换
// ============================================
Console.WriteLine("【10. 类型转换】");
Console.WriteLine(new string('-', 120));
Console.WriteLine("  ToList         .ToList() - 转为 List<T>");
Console.WriteLine("  ToDynamicList  .ToDynamicList() - 转为动态列表");
Console.WriteLine("  ToArray        .ToArray() - 转为数组");
Console.WriteLine("  ToDictionary   .ToDictionary(x => x.Id) - 转为字典");
Console.WriteLine("  Cast           .Cast<T>() - 类型转换");
Console.WriteLine("  OfType         .OfType<T>() - 筛选特定类型");
Console.WriteLine();

// ============================================
// 11. 量化表达式 (Any/All)
// ============================================
Console.WriteLine("【11. 量化表达式 (Any/All)】");
Console.WriteLine(new string('-', 120));
Console.WriteLine("  Any()          .Any() - 是否存在任何元素");
Console.WriteLine("  Any(condition) .Any(\"Price > 100\") - 是否存在满足条件的元素");
Console.WriteLine("  All(condition) .All(\"Price > 50\") - 所有元素都满足条件");
Console.WriteLine();

// ============================================
// 12. 连接操作 (Join)
// ============================================
Console.WriteLine("【12. 连接操作 (Join)】");
Console.WriteLine(new string('-', 120));
Console.WriteLine("  直接内连接");
Console.WriteLine("  var result = orders.Join(customers,");
Console.WriteLine("    o => o.CustomerId,");
Console.WriteLine("    c => c.Id,");
Console.WriteLine("    (o, c) => new { o.OrderId, c.CustomerName });");
Console.WriteLine();

// ============================================
// 13. 参数化查询示例
// ============================================
Console.WriteLine("【13. 参数化查询速查】");
Console.WriteLine(new string('-', 120));
Console.WriteLine("  单个参数      .Where(\"Age > @0\", 18)");
Console.WriteLine("  多个参数      .Where(\"City == @0 and Age > @1\", \"Paris\", 25)");
Console.WriteLine("  列表参数      .Where(\"@0.Contains(Status)\", new[] { \"Active\", \"Pending\" })");
Console.WriteLine("  日期参数      .Where(\"CreatedDate >= @0\", new DateTime(2024, 1, 1))");
Console.WriteLine();

// ============================================
// 14. 常见错误和注意事项
// ============================================
Console.WriteLine("【14. 常见错误和注意事项】");
Console.WriteLine(new string('-', 120));
Console.WriteLine("  ✗ 字段名不能用参数: .OrderBy(\"@0\", \"FieldName\") - 错误");
Console.WriteLine("  ✓ 字段名必须硬编码: .OrderBy(\"FieldName\") - 正确");
Console.WriteLine();
Console.WriteLine("  ✗ 字符串值需要转义: .Where(\"Name == 'John'\") - 可能出错");
Console.WriteLine("  ✓ 使用参数化查询: .Where(\"Name == @0\", \"John\") - 推荐");
Console.WriteLine();
Console.WriteLine("  ✗ 忘记调用 ToDynamicList: var list = query.Select(...); - 返回 IQueryable");
Console.WriteLine("  ✓ 正确调用: var list = query.Select(...).ToDynamicList(); - 返回 List");
Console.WriteLine();
Console.WriteLine("  ✗ 字段名拼写错误: .Where(\"Nmae == @0\", value) - 运行时异常");
Console.WriteLine("  ✓ 仔细检查字段名: .Where(\"Name == @0\", value) - 正确");
Console.WriteLine();

Console.WriteLine("\n【快速参考完成】");

public class StringExample { public string Text { get; set; } }
public class NumberExample { public int Value { get; set; } }
public class Item { public string Name { get; set; } public int Qty { get; set; } public decimal Price { get; set; } }
