<Query Kind="Statements">
  <NuGetReference Version="6.0.1">Microsoft.Extensions.Caching.Memory</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.Hosting</NuGetReference>
  <Namespace>Microsoft.Extensions.Caching.Memory</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>


// dotnet 类库中的 两个cache 实现。
//1 - System.Runtime.Caching/MemoryCache
//2 - Microsoft.Extensions.Caching.Memory / IMemoryCache

//1 - 用于 asp.net .net framework 不使用依赖注入
//2 - 用于 asp.net core 内部有依赖注入。

//public static IServiceCollection AddMemoryCache(this IServiceCollection services)
//{
//	if (services == null)
//	{
//		throw new ArgumentNullException("services");
//	}
//	services.AddOptions();
//	services.TryAdd(ServiceDescriptor.Singleton<IMemoryCache, MemoryCache>());
//	return services;
//}

IHost host = Host.CreateDefaultBuilder()
	.ConfigureServices((hostContext, services) =>
	{
		//依赖注入
		services.AddMemoryCache();
	})
	.Build();


var _memoryCache = host.Services.GetService<IMemoryCache>();

// 该方法将一个键值对添加到MemoryCache中，并设置相对过期时间。在指定的时间间隔过后，缓存项将被自动删除
_memoryCache.Set("key1","value1", TimeSpan.FromMinutes(30));

//该方法将一个键值对添加到MemoryCache中，并设置绝对过期时间。在指定的时间点，缓存项将被自动删除
_memoryCache.Set("key2", "value2", DateTimeOffset.Now.AddMinutes(10));

var cacheOptions = new MemoryCacheEntryOptions()
	.SetSlidingExpiration(TimeSpan.FromSeconds(5)) //5秒如果没有访问就过期,滑动过期不过超过绝对过期值。
	.SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(30)) // 两者过期任意满足就过期了。
	.SetPriority(CacheItemPriority.High)
	.SetSize(1);
_memoryCache.Set("key3", "value3", cacheOptions);


while (true)
{
	var value = _memoryCache.Get("key3").Dump();
	if (value == null)
		break;
	Thread.Sleep(10*1000);
}

_memoryCache.Get("key4").Dump();

host.Run();

