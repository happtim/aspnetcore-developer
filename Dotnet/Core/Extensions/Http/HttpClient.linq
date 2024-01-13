<Query Kind="Statements">
  <NuGetReference>CliWrap</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>CliWrap</Namespace>
  <Namespace>CliWrap.Buffered</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

//https://zhuanlan.zhihu.com/p/381738224
//1.开启Minimal api 

//HttpClient类使用比较简单，但在某些情况下，许多开发人员却并未正确使用该类；虽然此类实现 IDisposable，
//但在 using 语句中声明和实例化它并非首选操作，因为释放 HttpClient 对象时，基础套接字不会立即释放，
//这可能会导致套接字耗尽问题，最终可能会导致 SocketException 错误。要解决此问题，
//推荐的方法是将 HttpClient 对象创建为单一对象或静态对象

( await ( 
	Cli.Wrap("netstat").WithArguments("-ano") |  
	Cli.Wrap("findstr").WithArguments("TIME_WAIT"))
	.ExecuteBufferedAsync()
).StandardOutput.Dump();

for (int i = 0; i < 20; i++)
{
	using (HttpClient client = new HttpClient()) 
	{
		
		var result = await client.GetAsync("http://localhost:5000");
		var hello = await result.Content.ReadAsStringAsync();
		Console.WriteLine("收到信息：" + hello);
	}
}

(await(
	Cli.Wrap("netstat").WithArguments("-ano") |
	Cli.Wrap("findstr").WithArguments("TIME_WAIT"))
	.ExecuteBufferedAsync()
).StandardOutput.Dump();
