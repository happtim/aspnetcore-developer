<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
</Query>

void Main()
{
	// Newtonsoft.json 使用 JsonSerializer 类进行JsonText 和 Object 对象直接转化。
	// JsonConvert 提供了简单的JsonSerializer 包装 
	//
	// JsonConvert 下面源码显示 JsonConvert类序列化 还是使用的 JsonSerializer。
	//	public static string SerializeObject(object? value, Type? type, JsonSerializerSettings? settings)
	//  {
	//	  JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);
	//	  return SerializeObjectInternal(value, type, jsonSerializer);
	//  }

	Product product = new Product();
	
	product.Name = "Apple";
	product.ExpiryDate = new DateTime(2008, 12, 28);
	product.Price = 3.99M;
	product.Sizes = new string[] { "Small", "Medium", "Large" };
	
	string output = JsonConvert.SerializeObject(product,Newtonsoft.Json.Formatting.Indented); //使用缩进
	
	output.Dump();
	//{"Name":"Apple","ExpiryDate":"2008-12-28T00:00:00","Price":3.99,"Sizes":["Small","Medium","Large"]}
	
	Product deserializedProduct = JsonConvert.DeserializeObject<Product>(output);
	
	deserializedProduct.Dump();


	// JsonSerializer 提供了更多的控制，jsonText 到 Object之前的转化。
	// JsonSerializer 可以直通过JsonTextReader 和 JsonTextWriter 读写流。
	product = new Product();
	product.ExpiryDate = new DateTime(2008, 12, 28);
	JsonSerializer serializer = new JsonSerializer();
	serializer.Converters.Add(new JavaScriptDateTimeConverter()); //使用JavaScriptDatetime 方式转化
	serializer.NullValueHandling = NullValueHandling.Ignore;      //如果value为空则不转化。

	using (StringWriter sw = new StringWriter(new StringBuilder(256))) // 写字符串
	//using (StreamWriter sw = new StreamWriter(@"c:\json.txt"))       // 写文件
	using (JsonWriter writer = new JsonTextWriter(sw))
	{
		serializer.Serialize(writer, product);
		// {"ExpiryDate":new Date(1230393600000),"Price":0.0}
		sw.ToString().Dump();
	}
	
	
}

public class VssStorageChangeEto : List<VssStorageItem>
{
}

public class VssStorageItem
{
	public string Station { get; set; }
	public string Status { get; set; }
	public string StatusName { get; set; }
}

class Product
{
	public string Name {get;set;}
	public DateTime ExpiryDate {get;set;}
	public decimal Price {get;set;}
	public string [] Sizes {get;set;}
	
}