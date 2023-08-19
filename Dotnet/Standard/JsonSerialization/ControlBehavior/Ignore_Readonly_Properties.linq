<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>

#load ".\WeatherForecast"

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