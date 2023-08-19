<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>

#load ".\WeatherForecast"

var weatherForecast = new WeatherForecastWithIgnoreAttribute
{
	Date = DateTime.Parse("2019-08-01"),
	TemperatureCelsius = 25,
	Summary = "Hot",

};

var options = new JsonSerializerOptions { WriteIndented = true };
string jsonString = JsonSerializer.Serialize(weatherForecast, options);

jsonString.Dump();