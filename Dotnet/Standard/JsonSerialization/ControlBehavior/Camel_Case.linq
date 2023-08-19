<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

#load ".\WeatherForecast"

var weatherForecast = new WeatherForecastWithPropertyNameAttribute
{
	Date = DateTime.Now,
	TemperatureCelsius = 25,
	Summary = "Hot",
	WindSpeed = 35,
};

var serializeOptions = new JsonSerializerOptions
{
	PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
	WriteIndented = true
};

var jsonString = JsonSerializer.Serialize(weatherForecast, serializeOptions);

jsonString.Dump();

