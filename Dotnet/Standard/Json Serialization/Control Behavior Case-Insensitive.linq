<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>


var jsonString = 
@"{
  ""date"": ""2019-08-01T00:00:00-07:00"",
  ""temperatureCelsius"": 25,
  ""summary"": ""Hot""
}";

var options = new JsonSerializerOptions
{
	//默认大小敏感，如果设置成true则不敏感
	PropertyNameCaseInsensitive = true
};
var weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(jsonString, options);

weatherForecast.Dump();

public class WeatherForecast
{
	public DateTimeOffset Date { get; set; }
	public int TemperatureCelsius { get; set; }
	public string? Summary { get; set; }
}