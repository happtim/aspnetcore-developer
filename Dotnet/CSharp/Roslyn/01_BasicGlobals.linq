<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp.Scripting</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Scripting</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Scripting</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


"=== 示例 1: 基本的 ScriptGlobals 使用 ===".Dump();

// 定义全局变量类
var globals = new SimpleGlobals
{
	Name = "张三",
	Age = 25
};

// 执行脚本
var script = @"
	var greeting = $""你好, {Name}! 你今年 {Age} 岁。"";
	var nextYear = Age + 1;
	return $""{greeting} 明年你将 {nextYear} 岁。"";
";

try
{
	var result = await CSharpScript.EvaluateAsync<string>(script, globals: globals);
	result.Dump("结果");
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


// 简单的全局变量类
public class SimpleGlobals
{
	public string Name { get; set; }
	public int Age { get; set; }
}


