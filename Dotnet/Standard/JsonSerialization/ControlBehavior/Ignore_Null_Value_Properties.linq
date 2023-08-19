<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>

#load ".\WeatherForecast"

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