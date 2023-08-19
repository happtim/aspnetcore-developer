<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>

#load ".\WeatherForecast"

var weatherForecast = new WeatherForecastWithPropertyNameAttribute
{
	Date = DateTime.Now,
	TemperatureCelsius = 25,
	Summary = "Hot",
	WindSpeed = 35,
};

var options = new JsonSerializerOptions
{
	PropertyNamingPolicy = new UpperCaseNamingPolicy(),
	WriteIndented = true
};
var jsonString = JsonSerializer.Serialize(weatherForecast, options);

jsonString.Dump();


public class UpperCaseNamingPolicy : JsonNamingPolicy
{
	public override string ConvertName(string name) =>
		name.ToUpper();
}