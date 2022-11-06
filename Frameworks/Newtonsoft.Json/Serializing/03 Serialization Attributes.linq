<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Runtime.Serialization</Namespace>
</Query>

// Attribute （属性）可用于控制 Json.NET 序列化和反序列化 .NET 对象的方式。

// Standard .NET Serialization Attributes （标准 .NET 序列化属性）
// 除了使用内置的 Json.NET 属性外，Json.NET 还会查找 SerializableAttribute，DataContractAttribute，DataMemberAttribute，这些。net属性。

void Main()
{

	//JsonObjectAttribute 	
	// 此属性用于标记类行的序列化方式是opt-in ，还是opt-out模式。
	// opt-in：成员必须具有 JsonProperty 或 DataMember 属性才能序列化。
	// opt-out: 一切都是 默认情况下序列化，但可以使用 JsonIgnoreAttribute,所有公共fileds（字段），properties（属性）都序列化。私有忽略。
	//使用 DataContractAttribute 是另一种设置 opt-in模式的方式.
	Person person = new Person(){Name = "Tim ge",BirthDate = new DateTime(1991,04,28), LastModified = DateTime.Now};
	JsonConvert.SerializeObject(person).Dump();

	//JsonArrayAttribute/JsonDictionaryAttribute
	// JsonArrayAttribute和JsonDictionaryAttribute用于指定 类是否序列化为该集合类型。

	//JsonPropertyAttribute
	// JsonPropertyAttribute 有许多用途：
	//	默认情况下，JsonProperty 将与 .NET 属性同名。JsonPropertyAttribute允许自定义名称。
	// JsonPropertyAttribute 指示当成员序列化，当JsonObjectAttribute模式位Opt-in时。、
	// 它包括序列化和反序列化中的非公共属性。
	//它可用于自定义序列化属性名称的命名策略。
	//它可用于自定义属性的集合项 JsonConverter、类型名称处理和引用处理。
	//DataMemberAttribute 可以用作 JsonPropertyAttribute 的替代品。

	//JsonIgnoreAttribute
	//	从序列化中排除字段或属性。
	//NonSerializedAttribute可以用作 JsonIgnoreAttribute 的替代品。

	//JsonConverterAttribute
	//JsonConverterAttribute指定用于转换对象的JsonConverter。 
	//该属性可以放置在类或者属性上，放置在类上时，将使用指定的JsonConverter序列化类。[JsonConverter(typeof(PersonConverter))]
	//放置在属性上时，指定的JsonConverter 将用于转换属性。
	
	person.Status = UserStatus.Active;
	JsonConvert.SerializeObject(person, Newtonsoft.Json.Formatting.Indented).Dump();


	//JsonExtensionDataAttribute
	//JsonExtensionDataAttribute指示JsonSerializer反序列化 没有 匹配字段或属性的属性放入指定集合。

	string jsonExtensionDataJson = @"{
	  'NameAlias': 'Tim ge',
	  'Birth': '1991-04-28T00:00:00',
	  'LastModified': '2022-11-06T23:13:49.2273936+08:00',
	  'Department': null,
	  'Status': 'Active'
	}";
	
	//Birth 反序列化中 的字段属性 没有他。
	var jsonExtensionDataPerson = JsonConvert.DeserializeObject<Person>(jsonExtensionDataJson);
	jsonExtensionDataPerson.Dump();

	//JsonConstructorAttribute
	//JsonConstructorAttribute指示JsonSerializer在反序列化类时使用特定的构造函数。

	string jsonConstructorJson = @"{
	  'NameAlias': 'Tim ge',
	  'Birth': '1991-04-28T00:00:00',
	  'LastModified': '2022-11-06T23:13:49.2273936+08:00',
	  'Department': null,
	  'Status': 'Active',
	  'nickname':'xiang'
	}";

	var jsonConstructorPerson = JsonConvert.DeserializeObject<Person>(jsonConstructorJson);
	jsonConstructorPerson.Dump();
}

[JsonObject(MemberSerialization.OptOut)]
//[JsonConverter(typeof(PersonConverter))]
public class Person
{
	public Person(){}
	
	[JsonConstructor]
	public Person(string nickName)
	{
		_nickName = nickName;
	}
	
	// "John Smith"
	[JsonProperty("NameAlias")]
	//[JsonIgnoreAttribute]
	public string Name { get; set; }

	// "2000-12-15T22:11:03"
	[JsonProperty]
	public DateTime BirthDate { get; set; }

	// new Date(976918263055)
	[JsonProperty]
	public DateTime LastModified { get; set; }

	// not serialized because mode is opt-in
	public string Department { get; set; }

	private string _nickName ;//私有字段忽略

	public string NickName
	{
		get {return _nickName;} 
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public UserStatus Status { get; set; }

	[JsonExtensionData]
	private IDictionary<string, JToken> _additionalData = new Dictionary<string, JToken>();

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		// Birth is not deserialized to any property
		// and so it is added to the extension data dictionary
		string birthDate = (string)_additionalData["Birth"];
		BirthDate = DateTime.Parse(birthDate);
	}

}


public enum UserStatus
{
	NotConfirmed,
	Active,
	Deleted
}


public class PersonConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType  == typeof(Person);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		Person person = new Person();
		person.Name = (string)reader.Value;
		return person;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		Person person = (Person)value;
		writer.WritePropertyName("Name"); 
		writer.WriteValue(person.Name);
	}
}
