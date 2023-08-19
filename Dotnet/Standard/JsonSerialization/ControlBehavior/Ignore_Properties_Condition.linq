<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>

#load ".\WeatherForecast"

var weatherForecast = new WeatherForecastWithIgnoreCondition
{						 
	Date = default,
    Summary = null,
    TemperatureC = default

};

var options = new JsonSerializerOptions { WriteIndented = true };
string jsonString = JsonSerializer.Serialize(weatherForecast, options);

jsonString.Dump();