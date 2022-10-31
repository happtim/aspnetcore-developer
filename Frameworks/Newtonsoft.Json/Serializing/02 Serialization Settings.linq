<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

//JsonSerilizer 有很多属性可以序列化，在JsonConvert 也可以通过 JsonSerializerSettings 设置。
void Main()
{
	Product product = new Product();

	product.Name = "Apple";
	product.ExpiryDate = new DateTime(2008, 12, 28);
	product.Price = 3.99M;
	product.Sizes = new string[] { "Small", "Medium", "Large" };

	JsonSerializerSettings settings = new JsonSerializerSettings();
	
	// DateFormatHandling
	// DateFormatHandling.IsoDateFormat 默认使用使用ISO 8601 输出格式。 "2008-12-28T00:00:00"
	// DateFormatHandling.MicrosoftDateFormat 使用  Microsoft JSON 格式输出。 "\/Date(1230393600000+0800)\/"
	(new string('=', 10) + "DateFormatHandling" + new string('=', 10)).Dump();
	
 	settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
	var jsonDateFormant =JsonConvert.SerializeObject(product, Newtonsoft.Json.Formatting.Indented , settings);
	jsonDateFormant.Dump();

	// MissingMemberHandling
	// MissingMemberHandling.Ignore 默认使用 假如没有个转化的这个属性忽略异常。
	// MissingMemberHandling.Error 当没有属性报错。
	(new string('=', 10) + "MissingMemberHandling" + new string('=', 10)).Dump();
	string jsonMissingMember = @"{
			  'Name': 'Apple',
			  'ExpiryDate': '2008-12-28T00:00:00',
			  'Price': 3.99,
			  'Brand': 'Dole',
			  'Sizes': [
				'Small',
			    'Medium',
			    'Large'
			  ]
			}";
			
	settings.MissingMemberHandling = MissingMemberHandling.Ignore;
	var productMissingMember = JsonConvert.DeserializeObject<Product>(jsonMissingMember,settings);
	productMissingMember.Dump();

	// ReferenceLoopHandling 用来控制循环引用的处理方式
	// ReferenceLoopHandling.Error 默认选项 
	// ReferenceLoopHandling.Ignore 忽略循环的对象，对象第一次遇见会序列化，在子属性中在遇见就会忽略。
	// ReferenceLoopHandling.Serialize 输出循环引用，但是要求循环引用不是无限的。
	(new string('=', 10) + "ReferenceLoopHandling" + new string('=', 10)).Dump();
	settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
	product.SonProduct = product;
	var jsonReferenceLoop = JsonConvert.SerializeObject(product, Newtonsoft.Json.Formatting.Indented, settings);
	jsonReferenceLoop.Dump();

	// NullValueHandling 控制null值的处理。
	// NullValueHandling.Include 默认选项 序列化设置null给Json，反序列化设置null值给属性/字段。
	// NullValueHandling.Ignore 序列化为Json时，忽略null的字段。反序列化如果值为null忽略赋值给属性/字段。
	(new string('=', 10) + "NullValueHandling" + new string('=', 10)).Dump();
	string jsonNullValue = @"{
			  'Name': null,
			  'ExpiryDate': '2008-12-28T00:00:00',
			  'Price': 3.99,
			  'Sizes': [
				'Small',
			    'Medium',
			    'Large'
			  ]
			}";
	var productNullValue = JsonConvert.DeserializeObject<Product>(jsonNullValue, new JsonSerializerSettings()
	{
		NullValueHandling = NullValueHandling.Include
	});
	productNullValue.Dump();

	product.Name = null;
	jsonNullValue = JsonConvert.SerializeObject(product, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings()
	{
		NullValueHandling = NullValueHandling.Ignore,
		ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
	});
	jsonNullValue.Dump();

	// DefaultValueHandling 控制默认value序列化反序列化
	// DefaultValueHandling.Include 默认 当属性/字段值等于默认值时，都进行序列化反序列化
	// DefaultValueHandling.Ingore 当属性/字段值等于默认值时，不行进行序列化反序列化
	(new string('=', 10) + "DefaultValueHandling" + new string('=', 10)).Dump();
	settings.DefaultValueHandling = DefaultValueHandling.Include;
	string jsonDefaultValue = @"{
			  'Name': 'Apple',
			  'ExpiryDate': '2008-12-28T00:00:00',
			  'Price':0,
			  'Sizes': [
				'Small',
			    'Medium',
			    'Large'
			  ]
			}";

	var productDefaultValue = JsonConvert.DeserializeObject<Product>(jsonDefaultValue, new JsonSerializerSettings()
	{
		DefaultValueHandling = DefaultValueHandling.Include
	});
	productDefaultValue.Dump();
	
	jsonDefaultValue = JsonConvert.SerializeObject(productDefaultValue, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings() 
	{
		DefaultValueHandling = DefaultValueHandling.Ignore
	});
	jsonDefaultValue.Dump();
	
	
}



class Product
{
	public string Name { get; set; } = "Orange";
	public DateTime ExpiryDate { get; set; }
	public decimal Price { get; set; }  = 3.99M;
	public string[] Sizes { get; set; }
	public Product SonProduct {get;set;}

}
