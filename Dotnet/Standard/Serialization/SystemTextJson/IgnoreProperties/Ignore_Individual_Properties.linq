<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

//若要忽略单个属性，请使用 [JsonIgnore] 特性。

var weatherForecast = new WeatherForecastWithIgnoreAttribute
{
	Date = DateTime.Parse("2019-08-01"),
	TemperatureCelsius = 25,
	Summary = "Hot",

};

var options = new JsonSerializerOptions { WriteIndented = true };
string jsonString = JsonSerializer.Serialize(weatherForecast, options);

jsonString.Dump();

public class WeatherForecastWithIgnoreAttribute
{
	public DateTimeOffset Date { get; set; }
	public int TemperatureCelsius { get; set; }
	[JsonIgnore]
	public string? Summary { get; set; }
}