<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

//若要忽略所有 null 值属性，请将 DefaultIgnoreCondition 属性设置为 WhenWritingNull

var weatherForecast = new WeatherForecastWithROProperty
{
	Date = DateTime.Now,
	Summary = null,
	TemperatureCelsius = default,

};

var options = new JsonSerializerOptions
{
	DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
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