<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

//默认情况下，枚举会序列化为数字。 若要将枚举名称序列化为字符串，
//请使用 JsonStringEnumConverter 或 JsonStringEnumConverter<TEnum> 转换器。

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

//内置 JsonStringEnumConverter 还可以反序列化字符串值。 无论有没有指定的命名策略，它都能正常工作。

weatherForecast = JsonSerializer.Deserialize<WeatherForecastWithEnum>(jsonString, options);

//Dump 正常工作
weatherForecast.Dump();

public class WeatherForecastWithEnum
{
	public DateTimeOffset Date { get; set; }
	public int TemperatureCelsius { get; set; }
	public Summary? Summary { get; set; }
}

public enum Summary
{
	Cold, Cool, Warm, Hot
}

