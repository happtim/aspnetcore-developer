<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

//如果要序列化的对象的属性的类型为 Dictionary<string,TValue>，则可以使用命名策略（如 camel case）转换 string 键。

//字典键的命名策略仅适用于序列化。
var weatherForecast = new WeatherForecastWithPropertyNameAttribute
{
	Date = DateTime.Now,
	TemperatureCelsius = 25,
	Summary = "Hot",
	WindSpeed = 35,
	TemperatureRanges = new Dictionary<string,int> { {"ColdMinTemp",20 }, { "HotMinTemp",40 } }
};

var options = new JsonSerializerOptions
{
	DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
	WriteIndented = true
};

var jsonString = JsonSerializer.Serialize(weatherForecast, options);

jsonString.Dump();

public class WeatherForecastWithPropertyNameAttribute
{
	public DateTimeOffset Date { get; set; }
	public int TemperatureCelsius { get; set; }
	public string Summary { get; set; }
	[JsonPropertyName("Wind")]
	public int WindSpeed { get; set; }
	public Dictionary<string, int> TemperatureRanges { get; set; }
}