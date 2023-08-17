<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>


//默认情况下，属性名称匹配是区分大小写的。您可以指定大小写不敏感。
//如果JSON包含只读属性的值，默认情况下会被忽略。您可以将PreferredObjectCreationHandling选项设置为JsonObjectCreationHandling.Populate以启用反序列化读取只读属性。
//序列化器会忽略非公有构造函数。
//支持将数据反序列化为不可变对象或没有公共设置访问器的属性。请参阅不可变类型和记录。
//默认情况下，枚举类型以数字形式支持序列化。您可以将枚举值序列化为字符串。
//默认情况下，字段会被忽略。您可以包含字段。
//默认情况下，JSON中的注释或尾部逗号会引发异常。您可以允许注释和尾部逗号。
//默认的最大深度为64。

string jsonString =
@"{
  ""Date"": ""2019-08-01T00:00:00-07:00"",
  ""TemperatureCelsius"": 25,
  ""Summary"": ""Hot"",
  ""DatesAvailable"": [
    ""2019-08-01T00:00:00-07:00"",
    ""2019-08-02T00:00:00-07:00""
  ],
  ""TemperatureRanges"": {
                ""Cold"": {
                    ""High"": 20,
      ""Low"": -10
                },
    ""Hot"": {
                    ""High"": 60,
      ""Low"": 20
    }
            },
  ""SummaryWords"": [
    ""Cool"",
    ""Windy"",
    ""Humid""
  ]
}
";

WeatherForecast? weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(jsonString);
	
weatherForecast.Dump();

//异步读取
using MemoryStream memoryStream = new MemoryStream();
var buffer = System.Text.Encoding.UTF8.GetBytes(jsonString);
memoryStream.Write(buffer,0,buffer.Length);
memoryStream.Position = 0;
weatherForecast = await JsonSerializer.DeserializeAsync<WeatherForecast>(memoryStream);

weatherForecast.Dump();
await memoryStream.DisposeAsync();

public class WeatherForecast
{
	public DateTimeOffset Date { get; set; }
	public int TemperatureCelsius { get; set; }
	public string? Summary { get; set; }
	public string? SummaryField;
	public IList<DateTimeOffset>? DatesAvailable { get; set; }
	public Dictionary<string, HighLowTemps>? TemperatureRanges { get; set; }
	public string[]? SummaryWords { get; set; }
}

public class HighLowTemps
{
	public int High { get; set; }
	public int Low { get; set; }
}