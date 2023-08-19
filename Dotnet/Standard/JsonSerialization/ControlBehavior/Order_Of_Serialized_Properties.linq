<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>

#load ".\WeatherForecast"

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