<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

void Main()
{
	// Preserving Object References （保留对象引用）
	//默认情况下，Json.NET 将按值序列化它遇到的所有对象。 如果列表包含两个 Person 引用，并且两个引用都指向 相同的对象。JsonSerializer 将分别写出每个对象值。

	Person p = new Person
	{
		BirthDate = new DateTime(1980, 12, 23, 0, 0, 0, DateTimeKind.Utc),
		LastModified = new DateTime(2009, 2, 20, 12, 59, 21, DateTimeKind.Utc),
		Name = "James"
	};
	
	List<Person> people = new List<Person>();
	people.Add(p);
	people.Add(p);
	
	string json = JsonConvert.SerializeObject(people, Newtonsoft.Json.Formatting.Indented);
	//大多数情况下这是我们期望的结果，但在某种情况下将第二个值写为第一值的引用更好。
	json.Dump();


	//PreserveReferencesHandling （保留引用处理）
	json = JsonConvert.SerializeObject(people, Newtonsoft.Json.Formatting.Indented,
		new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
	json.Dump();

	List<Person> deserializedPeople = JsonConvert.DeserializeObject<List<Person>>(json,
		new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
		
	"person[0] == person[1]:".Dump();
	object.ReferenceEquals(deserializedPeople[0],deserializedPeople[1]).Dump();


	//IsReference
	// 用于细粒 控制应将哪些对象和成员序列化为 引用 JsonObjectAttribute ,JsonArrayAttribute 和 JsonPropertyAttribute 的 IsReference 属性。
	//将 JsonObjectAttribute 或 JsonArrayAttribute 上的 IsReference 设置为 true 将表示 JsonSerializer 将始终序列化类型 属性反对作为参考。
	//在 JsonPropertyAttribute 为 true 将仅将该属性序列化为 参考。
	EmployeeReference e = new EmployeeReference
	{
		BirthDate = new DateTime(1980, 12, 23, 0, 0, 0, DateTimeKind.Utc),
		LastModified = new DateTime(2009, 2, 20, 12, 59, 21, DateTimeKind.Utc),
		Name = "James"
	};
	List<EmployeeReference> employees = new List<EmployeeReference>();
	employees.Add(e);
	employees.Add(e);
 	json = JsonConvert.SerializeObject(employees, Newtonsoft.Json.Formatting.Indented);
	json.Dump();

	Employee manager = new Employee
	{
	    Name = "George-Michael"
	};
	Employee worker = new Employee
	{
	    Name = "Maeby",
	    Manager = manager
	};

	Business business = new Business
	{
	    Name = "Acme Ltd.",
	    Employees = new List<Employee>
	    {
	        manager,
	        worker
	    }
	};

	 json = JsonConvert.SerializeObject(business, Newtonsoft.Json.Formatting.Indented);
	 json.Dump();
	

}


class Person
{
	public string Name {get;set;}
	public DateTime LastModified { get; set; }
	public DateTime BirthDate {get;set;}
}

[JsonObject(IsReference = true)]
class EmployeeReference : Person
{
	public EmployeeReference Manager { get; set; }
}

public class Business
{
	public string Name { get; set; }

	[JsonProperty(ItemIsReference = true)]
	public IList<Employee> Employees { get; set; }
}

public class Employee
{
	public string Name { get; set; }

	[JsonProperty(IsReference = true)]
	public Employee Manager { get; set; }
}
