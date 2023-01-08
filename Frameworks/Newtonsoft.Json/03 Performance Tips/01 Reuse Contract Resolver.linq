<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
</Query>

//https://www.newtonsoft.com/json/help/html/Performance.htm

//有很多方法可以加快序列化加过

//IContractResolver 将 .NET 类型解析为 JsonSerializer 内部序列化期间使用的协定。
//创建协定涉及检查具有反射的类型会有开销，因此合约通常由 IContractResolver 的实现（如 DefaultContractResolver）缓存。

//若要避免每次使用 JsonSerializer 时重新创建协定的开销，应创建一次协定解析程序并重复使用它。
//请注意，如果您不使用协定解析程序，则在序列化和反序列化时会自动使用共享内部实例。


var person = new Person() {Name = "Time", BirthDate = DateTime.Now};

// BAD - a new contract resolver is created each time, forcing slow reflection to be used
string json1 = JsonConvert.SerializeObject(person, new JsonSerializerSettings
{
	Formatting = Newtonsoft.Json.Formatting.Indented,
	ContractResolver = new DefaultContractResolver
	{
		NamingStrategy = new SnakeCaseNamingStrategy()
	}
});
json1.Dump();


// GOOD - reuse the contract resolver from a shared location
string json2 = JsonConvert.SerializeObject(person, new JsonSerializerSettings
{
	Formatting = Newtonsoft.Json.Formatting.Indented,
	ContractResolver = AppSettings.SnakeCaseContractResolver
});

json2.Dump();

// GOOD - an internal contract resolver is used
string json3 = JsonConvert.SerializeObject(person, new JsonSerializerSettings
{
	Formatting = Newtonsoft.Json.Formatting.Indented
});

json3.Dump();

 static class AppSettings
{
	public static readonly IContractResolver SnakeCaseContractResolver = new DefaultContractResolver();
}

class Person
{
	public string Name { get; set; }
	public DateTime LastModified { get; set; }
	public DateTime BirthDate { get; set; }
}
