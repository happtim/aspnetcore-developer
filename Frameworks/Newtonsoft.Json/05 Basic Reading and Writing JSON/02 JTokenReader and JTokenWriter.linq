<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

//https://www.newtonsoft.com/json/help/html/ReadingWritingJSON.htm

//JTokenReader 和 JTokenWriter 读取和写入 LINQ 到 JSON 对象。它们位于 Newtonsoft.Json.Linq 命名空间中。
//这些对象允许您将 LINQ to JSON 对象与读取和写入 JSON 的对象（如 JsonSerializer）一起使用。
//例如，可以将 LINQ to JSON 对象反序列化为常规 .NET 对象，反之亦然。


JObject o = new JObject(
	new JProperty("Name", "John Smith"),
	new JProperty("BirthDate", new DateTime(1983, 3, 20))
	);

JsonSerializer serializer = new JsonSerializer();
Person p = (Person)serializer.Deserialize(new JTokenReader(o), typeof(Person));

p.Name.Dump();

class Person
{
	public string Name { get; set; }
	public DateTime LastModified { get; set; }
	public DateTime BirthDate { get; set; }
}

