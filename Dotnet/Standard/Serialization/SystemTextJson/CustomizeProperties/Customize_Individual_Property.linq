<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

//若要设置单个属性的名称，请使用 [JsonPropertyName] 特性。
// * 同时适用于两个方向（序列化和反序列化）。
// * 优先于属性命名策略。
// * 不影响参数化构造函数的参数名称匹配。
var weatherForecast = new WeatherForecastWithPropertyNameAttribute
{
	Date = DateTime.Now,
	TemperatureCelsius = 25,
	Summary = "Hot",
	WindSpeed = 35,
};

var options =  new JsonSerializerOptions 
{ 
	WriteIndented = true
};

var jsonString = JsonSerializer.Serialize(weatherForecast , options);

jsonString.Dump();

public class WeatherForecastWithPropertyNameAttribute
{
	public DateTimeOffset Date { get; set; }
	public int TemperatureCelsius { get; set; }
	public string Summary { get; set; }
	[JsonPropertyName("Wind")]
	public int WindSpeed { get; set; }
}


