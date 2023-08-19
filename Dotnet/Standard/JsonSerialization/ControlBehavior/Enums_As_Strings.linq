<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
</Query>

#load ".\WeatherForecast"

var weatherForecast = new WeatherForecastWithEnum
{
	Date = DateTime.Now,
	Summary = Summary.Hot,
	TemperatureCelsius = 25,
};

var options = new JsonSerializerOptions
{
	WriteIndented = true,
	Converters =
	{
		//new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
		new JsonStringEnumConverter()
	}
};

var jsonString = JsonSerializer.Serialize(weatherForecast, options);

jsonString.Dump();

weatherForecast = JsonSerializer.Deserialize<WeatherForecastWithEnum>(jsonString, options);

weatherForecast.Dump();

