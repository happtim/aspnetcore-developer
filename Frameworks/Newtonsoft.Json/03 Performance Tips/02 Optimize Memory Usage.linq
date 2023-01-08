<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>

// https://www.newtonsoft.com/json/help/html/Performance.htm#MemoryUsage

//若要使应用程序始终保持快速，请务必最大程度地减少.NET 框架执行垃圾回收所花费的时间。
//分配太多对象或分配非常大的对象可能会在垃圾回收过程中减慢甚至停止应用程序。

//为了最大程度地减少内存使用量和分配的对象数，Json.NET 支持直接序列化和反序列化到流。
//在处理大小大于 85kb 的 JSON 文档时，一次读取或写入一段 JSON，而不是将整个 JSON 字符串加载到内存中，
//以避免 JSON 字符串最终进入大型对象堆，这一点尤其重要。



HttpClient client = new HttpClient();

// read the json into a string
// string could potentially be very large and cause memory problems
string json = client.GetStringAsync("https://jsonplaceholder.typicode.com/posts").Result;

List<Person> persons = JsonConvert.DeserializeObject<List<Person>>(json);

persons.First().Dump();



client = new HttpClient();

using (Stream s = client.GetStreamAsync("https://jsonplaceholder.typicode.com/posts").Result)
using (StreamReader sr = new StreamReader(s))
using (JsonReader reader = new JsonTextReader(sr))
{
	JsonSerializer serializer = new JsonSerializer();

	// read the json from a stream
	// json size doesn't matter because only a small piece is read at a time from the HTTP request
	persons = JsonConvert.DeserializeObject<List<Person>>(json);

	persons.First().Dump();
}


public class Person 
{
	public int UserId {get;set;}
	public int Id {get;set;}
	public string Title {get;set;}
	public string Body {get;set;}
} 