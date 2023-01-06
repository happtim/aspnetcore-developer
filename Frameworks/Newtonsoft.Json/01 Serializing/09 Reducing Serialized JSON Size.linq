<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Runtime.Serialization</Namespace>
  <Namespace>System.ComponentModel</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
</Query>

//Reducing Serialized JSON Size （减小序列化 JSON 大小）
//将 .NET 对象序列化为 时遇到的常见问题之一 JSON是JSON最终包含许多不需要的属性和值。 
//在将 JSON 返回到客户端时，这一点尤其重要。更多 JSON 意味着更多的带宽和更慢的网站。


void Main()
{
	//JsonIgnoreAttribute 和 DataMemberAttribute
	//  默认情况下，Json.NET 将包含类的所有公共属性和字段 在它创建的 JSON 中。
	//  将JsonIgnoreAttribute添加到属性会告诉序列化程序始终跳过将其写入 JSON 结果。

	Car car = new Car{
		Model  = "Mazda",
		Year = new DateTime(1991,01,01),
		Features = new List<string>{"座椅加热","座椅通风"},
		LastModified =  new DateTime(2022,01,01),
	};
	JsonConvert.SerializeObject(car,Newtonsoft.Json.Formatting.Indented).Dump();
	
	//如果一个类具有许多属性，并且您只想序列化一小部分 其中，将 JsonIgnore 添加到所有其他将很乏味且容易出错。
	//处理此方案的方法是将DataContractAttribute添加到类中，并将DataMemberAttribute添加到要序列化的属性中。只有您标记的属性才会被序列化。
	Computer computer = new Computer{
		Name = "ASUS",
		SalePrice = 12.23m,
		Manufacture = "上海",
		StockCount = 10,
		WholeSalePrice = 32.23m,
		NextShipmentDate = new DateTime(2012,02,18)
	};
	
	JsonConvert.SerializeObject(computer,Newtonsoft.Json.Formatting.Indented).Dump();

	//Formatting
	//由序列化程序编写的 JSON，其中"Formatting"选项设置为"Indented"生成格式良好、易于阅读的 JSON。
	//Formatting.None，使 JSON 结果保持较小，跳过 所有不必要的空格和换行符，以产生最紧凑和高效的 JSON 可能。

	//NullValueHandling 空值处理
	// NullValueHandling是 JsonSerializer 上的一个选项，用于控制 序列化程序处理具有 null 值的属性。
	// 通过将值设置为 NullValueHandling.忽略 JsonSerializer 跳过写入任何具有 值为空。

	Movie movie = new Movie();
	movie.Name = "Bad Boys III";
	movie.Description = "It's no Bad Boys";

	JsonConvert.SerializeObject(movie,
		Newtonsoft.Json.Formatting.Indented,
		new JsonSerializerSettings { }).Dump();

	JsonConvert.SerializeObject(movie,
		Newtonsoft.Json.Formatting.Indented,
		new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }).Dump();

	//DefaultValueHandling 默认值处理
	// DefaultValueHandling是 JsonSerializer 上的一个选项，用于控制序列化程序如何处理 具有默认值的属性。
	// 设置值 DefaultValueHandling.Ignore 将使 JsonSerializer 跳过写入任何具有默认值的属性 值。

	Invoice invoice = new Invoice
	{
		Company = "Acme Ltd.",
		Amount = 50.0m,
		Paid = false,
		FollowUpDays = 30,
		FollowUpEmailAddress = string.Empty,
		PaidDate = null
	};

	string included = JsonConvert.SerializeObject(invoice,
		Newtonsoft.Json.Formatting.Indented,
		new JsonSerializerSettings { }).Dump();

	string ignored = JsonConvert.SerializeObject(invoice,
		Newtonsoft.Json.Formatting.Indented,
		new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore }).Dump();

	//IContractResolver 
	//为了获得更大的灵活性，IContractResolver 提供了一个用于自定义的接口 如何将 .NET 对象序列化为 JSON 的几乎所有方面，包括更改 运行时的序列化行为。

	Book book = new Book
	{
		BookName = "The Gathering Storm",
		BookPrice = 16.19m,
		AuthorName = "Brandon Sanderson",
		AuthorAge = 34,
		AuthorCountry = "United States of America"
	};

	string startingWithA = JsonConvert.SerializeObject(book, Newtonsoft.Json.Formatting.Indented,
		new JsonSerializerSettings { ContractResolver = new DynamicContractResolver('A') }).Dump();

	string startingWithB = JsonConvert.SerializeObject(book, Newtonsoft.Json.Formatting.Indented,
		new JsonSerializerSettings { ContractResolver = new DynamicContractResolver('B') }).Dump();
}


public class Car
{
	// included in JSON
	public string Model { get; set; }
	public DateTime Year { get; set; }
	public List<string> Features { get; set; }

	// ignored
	[JsonIgnore]
	public DateTime LastModified { get; set; }
}

[DataContract]
public class Computer
{
	// included in JSON
	[DataMember]
	public string Name { get; set; }

	[DataMember]
	public decimal SalePrice { get; set; }

	// ignored
	public string Manufacture { get; set; }
	public int StockCount { get; set; }
	public decimal WholeSalePrice { get; set; }
	public DateTime NextShipmentDate { get; set; }
}


public class Movie
{
	public string Name { get; set; }
	public string Description { get; set; }
	public string Classification { get; set; }
	public string Studio { get; set; }
	public DateTime? ReleaseDate { get; set; }
	public List<string> ReleaseCountries { get; set; }
}

public class Invoice
{
	public string Company { get; set; }
	public decimal Amount { get; set; }

	// false is default value of bool
	public bool Paid { get; set; }
	// null is default value of nullable
	public DateTime? PaidDate { get; set; }

	// customize default values
	[DefaultValue(30)]
	public int FollowUpDays { get; set; }

	[DefaultValue("")]
	public string FollowUpEmailAddress { get; set; }
}

public class DynamicContractResolver : DefaultContractResolver
{
	private readonly char _startingWithChar;

	public DynamicContractResolver(char startingWithChar)
	{
		_startingWithChar = startingWithChar;
	}

	protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
	{
		IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

		// only serializer properties that start with the specified character
		properties =
			properties.Where(p => p.PropertyName.StartsWith(_startingWithChar.ToString())).ToList();

		return properties;
	}
}

public class Book
{
	public string BookName { get; set; }
	public decimal BookPrice { get; set; }
	public string AuthorName { get; set; }
	public int AuthorAge { get; set; }
	public string AuthorCountry { get; set; }
}
