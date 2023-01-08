<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

//https://www.newtonsoft.com/json/help/html/ReadingWritingJSON.htm

//要手动读取和写入 JSON，Json.NET 提供了 JsonReader 和 JsonWriter 类。

//JsonReader 和 JsonWriter 是低级类，主要供 Json.NET 内部使用。
//若要快速使用 JSON，建议使用序列化程序 - 序列化和反序列化 JSON，或使用 LINQ to JSON。


StringBuilder sb = new StringBuilder();
StringWriter sw = new StringWriter(sb);

using (JsonWriter writer = new JsonTextWriter(sw))
{
	writer.Formatting = Newtonsoft.Json.Formatting.Indented;

	writer.WriteStartObject();
	writer.WritePropertyName("CPU");
	writer.WriteValue("Intel");
	writer.WritePropertyName("PSU");
	writer.WriteValue("500W");
	writer.WritePropertyName("Drives");
	writer.WriteStartArray();
	writer.WriteValue("DVD read/writer");
	writer.WriteComment("(broken)");
	writer.WriteValue("500 gigabyte hard drive");
	writer.WriteValue("200 gigabyte hard drive");
	writer.WriteEnd();
	writer.WriteEndObject();
	
}

var json =  sb.ToString();
json.Dump();

JsonTextReader reader = new JsonTextReader(new StringReader(json));
while (reader.Read())
{
	if (reader.Value != null)
	{
		Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
	}
	else
	{
		Console.WriteLine("Token: {0}", reader.TokenType);
	}
}


