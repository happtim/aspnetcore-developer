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

var options =  new JsonSerializerOptions 
{ 
	WriteIndented = true
};

var jsonString = JsonSerializer.Serialize(weatherForecast , options);

jsonString.Dump();


