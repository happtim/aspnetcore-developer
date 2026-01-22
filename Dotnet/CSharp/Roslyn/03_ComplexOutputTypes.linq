<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp.Scripting</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Scripting</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Scripting</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


	"=== 示例 3: 传出复杂类型 ===".Dump();
	
	var globals = new OutputGlobals
	{
		ProductName = "智能手机",
		BasePrice = 3999.99m,
		DiscountRate = 0.15m
	};

// 脚本返回复杂对象
var script = @"
	var finalPrice = BasePrice * (1 - DiscountRate);
	var savings = BasePrice - finalPrice;
	
	// 创建并返回复杂对象
	return new PriceCalculation
	{
		ProductName = ProductName,
		OriginalPrice = BasePrice,
		DiscountRate = DiscountRate,
		FinalPrice = finalPrice,
		Savings = savings,
		CalculatedAt = DateTime.Now
	};
";

var options = ScriptOptions.Default
	.AddReferences(typeof(PriceCalculation).Assembly)
	.AddImports("System");

try
{
	var result = await CSharpScript.EvaluateAsync<PriceCalculation>(script, options, globals);
	result.Dump("价格计算结果");
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


// 用于输出的全局变量
public class OutputGlobals
{
	public string ProductName { get; set; }
	public decimal BasePrice { get; set; }
	public decimal DiscountRate { get; set; }
}



public class PriceCalculation
{
	public string ProductName { get; set; }
	public decimal OriginalPrice { get; set; }
	public decimal DiscountRate { get; set; }
	public decimal FinalPrice { get; set; }
	public decimal Savings { get; set; }
	public DateTime CalculatedAt { get; set; }
}

