<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>

//如果属性包含公共 getter 而不是公共 setter，则属性为只读。
// 若要在序列化时忽略所有只读属性，请将 JsonSerializerOptions.IgnoreReadOnlyProperties 设置为 true
var weatherForecast = new WeatherForecastWithROProperty
{
	Date = DateTime.Now,
	Summary = "Hot",
	TemperatureCelsius = 25

};

var options = new JsonSerializerOptions
{
	IgnoreReadOnlyProperties = true,
	WriteIndented = true
};
var jsonString = JsonSerializer.Serialize(weatherForecast, options);

jsonString.Dump();

public class WeatherForecastWithROProperty
{
	public DateTimeOffset Date { get; set; }
	public int TemperatureCelsius { get; set; }
	public string? Summary { get; set; }
	public int WindSpeedReadOnly { get; private set; } = 35;
}