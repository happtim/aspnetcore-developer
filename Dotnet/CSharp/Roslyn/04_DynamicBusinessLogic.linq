<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp.Scripting</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Scripting</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Scripting</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


"=== 示例 5: 动态编译和执行业务逻辑 ===".Dump();

// 从数据库或配置中加载的动态规则
var ruleScript = @"
	// 业务规则：订单折扣计算
	decimal CalculateDiscount(Order order, Customer customer)
	{
		decimal discount = 0m;
		
		// 规则1: VIP客户额外折扣
		if (customer.IsVIP)
		{
			discount += 0.10m;
		}
		
		// 规则2: 大额订单折扣
		var totalAmount = order.Quantity * order.Price;
		if (totalAmount > 10000m)
		{
			discount += 0.05m;
		}
		else if (totalAmount > 5000m)
		{
			discount += 0.03m;
		}
		
		// 规则3: 季节性促销
		var month = DateTime.Now.Month;
		if (month == 11 || month == 12) // 双11、双12
		{
			discount += 0.15m;
		}
		
		// 最大折扣限制
		return Math.Min(discount, 0.30m);
	}
	
	// 应用折扣
	var discount = CalculateDiscount(CurrentOrder, CurrentCustomer);
	var originalTotal = CurrentOrder.Quantity * CurrentOrder.Price;
	var discountAmount = originalTotal * discount;
	var finalTotal = originalTotal - discountAmount;
	
	return new DiscountResult
	{
		OriginalAmount = originalTotal,
		DiscountRate = discount,
		DiscountAmount = discountAmount,
		FinalAmount = finalTotal,
		AppliedRules = new List<string>
		{
			CurrentCustomer.IsVIP ? ""VIP客户折扣"" : null,
			originalTotal > 10000m ? ""大额订单折扣(>10000)"" : 
				originalTotal > 5000m ? ""大额订单折扣(>5000)"" : null,
			(DateTime.Now.Month == 11 || DateTime.Now.Month == 12) ? ""季节性促销"" : null
		}.Where(r => r != null).ToList()
	};
";

var globals = new DynamicRuleGlobals
{
	CurrentOrder = new Order
	{
		Id = 201,
		ProductName = "高端服务器",
		Quantity = 2,
		Price = 8999.99m
	},
	CurrentCustomer = new Customer
	{
		Id = 2,
		Name = "赵六",
		Email = "zhaoliu@example.com",
		IsVIP = true
	}
};

var options = ScriptOptions.Default
	.AddReferences(typeof(Order).Assembly)
	.AddReferences(typeof(System.Linq.Enumerable).Assembly)
	.AddImports("System", "System.Linq", "System.Collections.Generic");

try
{
	var result = await CSharpScript.EvaluateAsync<DiscountResult>(ruleScript, options, globals);
	result.Dump("折扣计算结果");
}
catch (CompilationErrorException e)
{
	Console.WriteLine("脚本编译失败:");
	Console.WriteLine(string.Join(Environment.NewLine, e.Diagnostics));
}
catch (Exception e)
{
	Console.WriteLine($"脚本执行出错: {e.Message}");
}

// 展示如何缓存和重用编译后的脚本
"\n--- 重用编译后的脚本 ---".Dump();
var compiledScript = CSharpScript.Create<DiscountResult>(ruleScript, options, typeof(DynamicRuleGlobals));

// 第二个订单
var globals2 = new DynamicRuleGlobals
{
	CurrentOrder = new Order
	{
		Id = 202,
		ProductName = "办公桌椅套装",
		Quantity = 5,
		Price = 1299.99m
	},
	CurrentCustomer = new Customer
	{
		Id = 3,
		Name = "孙七",
		Email = "sunqi@example.com",
		IsVIP = false
	}
};

try
{
	var result2 = await compiledScript.RunAsync(globals2);
	result2.ReturnValue.Dump("第二个订单的折扣计算");
}
catch (CompilationErrorException e)
{
	Console.WriteLine("脚本编译失败:");
	Console.WriteLine(string.Join(Environment.NewLine, e.Diagnostics));
}
catch (Exception e)
{
	Console.WriteLine($"脚本执行出错: {e.Message}");
}


#region 全局变量类定义

// 动态规则的全局变量
public class DynamicRuleGlobals
{
	public Order CurrentOrder { get; set; }
	public Customer CurrentCustomer { get; set; }
}

#endregion

#region 数据模型类

public class Customer
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public bool IsVIP { get; set; }
}

public class Order
{
	public int Id { get; set; }
	public string ProductName { get; set; }
	public int Quantity { get; set; }
	public decimal Price { get; set; }
}

#endregion

#region 输出结果类

public class DiscountResult
{
	public decimal OriginalAmount { get; set; }
	public decimal DiscountRate { get; set; }
	public decimal DiscountAmount { get; set; }
	public decimal FinalAmount { get; set; }
	public List<string> AppliedRules { get; set; }
}

#endregion
