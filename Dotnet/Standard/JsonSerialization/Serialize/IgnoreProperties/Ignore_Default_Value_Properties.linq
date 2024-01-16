<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

//若要防止对值类型属性中的默认值进行序列化，请将 DefaultIgnoreCondition 属性设置为 WhenWritingDefault，

var weatherForecast = new WeatherForecastWithROProperty
{
	Date = DateTime.Now,
	Summary = null,
	TemperatureCelsius = default,

};

var options = new JsonSerializerOptions
{
	DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
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