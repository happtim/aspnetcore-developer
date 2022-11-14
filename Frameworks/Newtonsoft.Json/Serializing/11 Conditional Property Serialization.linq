<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
</Query>

//Conditional Property Serialization （条件属性序列化）

void Main()
{
	// 若要有条件地序列化属性，请添加一个返回与属性同名的布尔值的方法，然后在方法名称前面加上前缀 与应该序列化。
 	// 该方法的结果确定是否序列化属性。
 	// 如果该方法返回 true，则 属性将被序列化，如果它返回 false，则将跳过该属性。
	Employee joe = new Employee();
	joe.Name = "Joe Employee";
	Employee mike = new Employee();
	mike.Name = "Mike Manager";

	joe.Manager = mike;

	// mike is his own manager
	// ShouldSerialize will skip this property
	mike.Manager = mike;

	//string json = JsonConvert.SerializeObject(new[] { joe, mike }, Newtonsoft.Json.Formatting.Indented).Dump();
	
	
	//IContractResolver 
	//  如果您不想将 ShouldSerialize 方法放在类上，则使用 IContractResolver 有条件地序列化属性很有用 或者您没有声明类并且无法声明。
	JsonConvert.SerializeObject(new[] { joe, mike }, 
		Newtonsoft.Json.Formatting.Indented,
		new JsonSerializerSettings{ ContractResolver = new ShouldSerializeContractResolver()}
		).Dump();
}

public class Employee
{
	public string Name { get; set; }
	public Employee Manager { get; set; }

	//public bool ShouldSerializeManager()
	//{
	//	// 如果自己是管理着，那么就补序列化则个。
	//	return (Manager != this);
	//}
}

public class ShouldSerializeContractResolver : DefaultContractResolver
{
	//public new static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

	protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
	{
		JsonProperty property = base.CreateProperty(member, memberSerialization);

		if (property.DeclaringType == typeof(Employee) && property.PropertyName == "Manager")
		{
			property.ShouldSerialize =
				instance =>
				{
					Employee e = (Employee)instance;
					return e.Manager != e;
				};
		}

		return property;
	}
}
