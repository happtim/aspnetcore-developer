<Query Kind="Statements">
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

// 创建依赖注入容器
var serviceProvider = new ServiceCollection()
	.AddTransient<MyClass>() // 注册 MyClass 类型的依赖注入服务
	.BuildServiceProvider();

// 使用 ActivatorUtilities.CreateInstance 创建 MyClass 的实例并进行依赖注入
var myInstance = ActivatorUtilities.CreateInstance<MyClass>(serviceProvider, 42);

Console.WriteLine($"MyProperty: {myInstance.MyProperty}");


public class MyClass
{
	public int MyProperty { get; set; }

	public MyClass(int myPropertyValue)
	{
		MyProperty = myPropertyValue;
	}
}