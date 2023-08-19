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
	TemperatureRanges = new Dictionary<string,int> { {"ColdMinTemp",20 }, { "HotMinTemp",40 } }
};

var options = new JsonSerializerOptions
{
	DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
	WriteIndented = true
};

var jsonString = JsonSerializer.Serialize(weatherForecast, options);

jsonString.Dump();