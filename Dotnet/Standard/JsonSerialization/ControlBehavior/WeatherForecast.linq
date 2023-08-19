<Query Kind="Statements">
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

public class WeatherForecastWithPropertyNameAttribute
{
	public DateTimeOffset Date { get; set; }
	public int TemperatureCelsius { get; set; }
	public string Summary { get; set; }
	[JsonPropertyName("Wind")]
	public int WindSpeed { get; set; }
	public Dictionary<string,int> TemperatureRanges { get;set; }
}


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

public class WeatherForecastWithOrder
{
	[JsonPropertyOrder(-5)]
	public DateTime Date { get; set; }
	public int TemperatureC { get; set; }
	[JsonPropertyOrder(-2)]
	public int TemperatureF { get; set; }
	[JsonPropertyOrder(5)]
	public string? Summary { get; set; }
	[JsonPropertyOrder(2)]
	public int WindSpeed { get; set; }
}

public class WeatherForecastWithIgnoreAttribute
{
	public DateTimeOffset Date { get; set; }
	public int TemperatureCelsius { get; set; }
	[JsonIgnore]
	public string? Summary { get; set; }
}

public class WeatherForecastWithIgnoreCondition
{
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public DateTime Date { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.Never)]
	public int TemperatureC { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Summary { get; set; }
};

public class WeatherForecastWithROProperty
{
	public DateTimeOffset Date { get; set; }
	public int TemperatureCelsius { get; set; }
	public string? Summary { get; set; }
	public int WindSpeedReadOnly { get; private set; } = 35;
}