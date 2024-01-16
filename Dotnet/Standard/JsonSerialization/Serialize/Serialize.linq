<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>


//默认情况下，所有公共属性都会被序列化。您可以指定 [JsonIgnore] 要忽略的属性。 
//默认编码器会对非ASCII字符、ASCII范围内的HTML敏感字符以及根据RFC 8259 JSON规范进行转义的字符进行转义。
//默认情况下，JSON是被压缩的。您可以对JSON进行漂亮的打印输出。 
//默认情况下，JSON名称的大小写与.NET名称匹配。您可以自定义JSON名称的大小写。 
//默认情况下，会检测到环形引用并抛出异常。您可以保留引用并处理环形引用。
//默认情况下，会忽略字段。您可以包含字段。

var weatherForecast = new WeatherForecast
{
	Date = DateTime.Parse("2019-08-01"),
	TemperatureCelsius = 25,
	Summary = "Hot"
};

//同步方法
string jsonString = JsonSerializer.Serialize(weatherForecast);

jsonString.Dump();

//异步方法
using MemoryStream createStream = new MemoryStream();

// 创建一个StreamReader用于读取流中的数据
using StreamReader streamReader = new StreamReader(createStream);

await JsonSerializer.SerializeAsync(createStream, weatherForecast);

createStream.Seek(0, SeekOrigin.Begin);

streamReader.ReadToEnd().Dump();

//序列化为格式化 JSON
jsonString = JsonSerializer.Serialize<WeatherForecast>(weatherForecast, new JsonSerializerOptions { WriteIndented = true});

jsonString.Dump();


public class WeatherForecast
{
	public DateTimeOffset Date { get; set; }
	public int TemperatureCelsius { get; set; }
	public string? Summary { get; set; }
}