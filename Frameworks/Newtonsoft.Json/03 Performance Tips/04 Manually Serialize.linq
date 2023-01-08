<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

// https://www.newtonsoft.com/json/help/html/Performance.htm

//读取和写入 JSON 的绝对最快的方法是直接使用 JsonTextReader/JsonTextWriter 手动序列化类型。
//使用读取器或编写器可直接跳过序列化程序的任何开销，例如反射。

Person person = new Person(){Name = "Tim"};
person.Likes.Add("Program");

var json = person.ToJson();
json.Dump();

var p =  json.ToPerson();
p.Dump();

public class Person
{
	public Person()
	{
		Likes = new List<string>();
	}

	public string Name { get; set; }
	public IList<string> Likes { get; private set; }
}

public static class PersonWriter
{
	public static string ToJson(this Person p)
	{
		StringWriter sw = new StringWriter();
		JsonTextWriter writer = new JsonTextWriter(sw);

		// {
		writer.WriteStartObject();

		// "name" : "Jerry"
		writer.WritePropertyName("name");
		writer.WriteValue(p.Name);

		// "likes": ["Comedy", "Superman"]
		writer.WritePropertyName("likes");
		writer.WriteStartArray();
		foreach (string like in p.Likes)
		{
			writer.WriteValue(like);
		}
		writer.WriteEndArray();

		// }
		writer.WriteEndObject();

		return sw.ToString();
	}


	public static Person ToPerson(this string s)
	{
		StringReader sr = new StringReader(s);
		JsonTextReader reader = new JsonTextReader(sr);

		Person p = new Person();

		// {
		reader.Read();
		// "name"
		reader.Read();
		// "Jerry"
		p.Name = reader.ReadAsString();
		// "likes"
		reader.Read();
		// [
		reader.Read();
		// "Comedy", "Superman", ]
		while (reader.Read() && reader.TokenType != JsonToken.EndArray)
		{
			p.Likes.Add((string)reader.Value);
		}
		// }
		reader.Read();

		return p;
	}
}