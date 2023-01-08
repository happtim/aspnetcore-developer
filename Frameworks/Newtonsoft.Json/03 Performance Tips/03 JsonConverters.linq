<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

//https://www.newtonsoft.com/json/help/html/Performance.htm

//将 JsonConverter 传递给 SerializeObject 或 DeserializeObject 提供了一种完全更改对象序列化方式的简单方法。
//但是，有少量的开销;为每个值调用 CanConvert 方法，以检查序列化是否应由该 JsonConverter 处理。

//有几种方法可以在没有任何开销的情况下继续使用 JsonConverters。
//最简单的方法是使用 JsonConverterAttribute 指定 JsonConverter。
//此属性告知序列化程序在序列化和反序列化类型时始终使用该转换器，而无需进行检查。

var json = "{'Name':'Tim'}";
Person person =  JsonConvert.DeserializeObject<Person>(json);
//Person person =  JsonConvert.DeserializeObject<Person>(json,new PersonConverter());
person.Dump();


//如果要转换的类不是您自己的类，并且无法使用属性，则仍可以通过创建自己的 IContractResolve 程序来使用 JsonConverter。
json = JsonConvert.SerializeObject(new DateTime(2000, 10, 10, 10, 10, 10, DateTimeKind.Utc), new JsonSerializerSettings
{
	ContractResolver = ConverterContractResolver.Instance
});

json.Dump();

public class PersonConverter : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		JObject o = (JObject)JToken.ReadFrom(reader);

		Person p = new Person
		{
			Name = (string)o["Name"]
		};

		return p;
	}

	public override bool CanConvert(Type objectType)
	{
		return (objectType == typeof(Person));
	}
}

[JsonConverter(typeof(PersonConverter))]
public class Person
{
	public Person()
	{
		Likes = new List<string>();
	}

	public string Name { get; set; }
	public IList<string> Likes { get; private set; }
}


public class ConverterContractResolver : DefaultContractResolver
{
	public new static readonly ConverterContractResolver Instance = new ConverterContractResolver();

	protected override JsonContract CreateContract(Type objectType)
	{
		JsonContract contract = base.CreateContract(objectType);

		// this will only be called once and then cached
		if (objectType == typeof(DateTime) || objectType == typeof(DateTimeOffset))
		{
			contract.Converter = new JavaScriptDateTimeConverter();
		}

		return contract;
	}
}