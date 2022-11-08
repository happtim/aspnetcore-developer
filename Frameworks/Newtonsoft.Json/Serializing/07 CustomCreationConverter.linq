<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
</Query>

void Main()
{
	// CustomCreationConverter （自定义创建转换器）
	// CustomCreationConverter 是一个JsonConverter  它提供了一种方式 自定义在 JSON 反序列化期间创建对象的方式。

	string json =
	@"[
	  {
	    'FirstName': 'Maurice',
	    'LastName': 'Moss',
	    'BirthDate': '1981-03-08T00:00Z',
	    'Department': 'IT',
	    'JobTitle': 'Support'
	  },
	  {
	    'FirstName': 'Jen',
	    'LastName': 'Barber',
	    'BirthDate': '1985-12-10T00:00Z',
	    'Department': 'IT',
	    'JobTitle': 'Manager'
	  }
	]";
	List < IPerson > people = JsonConvert.DeserializeObject<List<IPerson>>(json, new PersonConverter());
	people.Dump();


}


public interface IPerson
{
	string FirstName { get; set; }
	string LastName { get; set; }
	DateTime BirthDate { get; set; }
}

public class Employee : IPerson
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public DateTime BirthDate { get; set; }

	public string Department { get; set; }
	public string JobTitle { get; set; }
}

public class PersonConverter : CustomCreationConverter<IPerson>
{
	public override IPerson Create(Type objectType)
	{
		return new Employee();
	}
}
