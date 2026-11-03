<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

//默认情况下，属性按在类中定义的顺序进行序列化。 通过 [JsonPropertyOrder] 特性，可指定序列化的 JSON 输出中的属性顺序。
//Order 属性的默认值是零。 
//如果将 Order 设置为正数，则会将属性放置在具有默认值的属性后面。 
//如果 Order 是负数，则会将属性放置在具有默认值的属性前面。
//属性按 Order 值从小到大的顺序编写的。
var weatherForecast = new WeatherForecastWithOrder
{
	Date = DateTime.Parse("2019-08-01"),
	TemperatureC = 25,
	TemperatureF = 25,
	Summary = "Hot",
	WindSpeed = 10
};

var options = new JsonSerializerOptions { WriteIndented = true };
string jsonString = JsonSerializer.Serialize(weatherForecast, options);

jsonString.Dump();

public class WeatherForecastWithOrder
{
	[JsonPropertyOrder(-5)]
	public DateTime Date { get; set; }
	public int TemperatureC { get; set; }
	[JsonPropertyOrder(-2)]
	public int TemperatureF { get; set; }
	[JsonPropertyOrder(5)]
	public string? Summary { get; set; }
	[JsonPropertyOrder(2)]
	public int WindSpeed { get; set; }
}