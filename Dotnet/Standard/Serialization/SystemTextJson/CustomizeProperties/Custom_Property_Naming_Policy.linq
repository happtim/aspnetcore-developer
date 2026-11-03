<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

//若要使用自定义 JSON 属性命名策略，请创建派生自 JsonNamingPolicy 的类，并替代 ConvertName 方法，

// * 适用于序列化和反序列化。
// * 由[JsonPropertyName] 特性替代。 这便是示例中的 JSON 属性名称 Wind 不是大写的原因。
var weatherForecast = new WeatherForecastWithPropertyNameAttribute
{
	Date = DateTime.Now,
	TemperatureCelsius = 25,
	Summary = "Hot",
	WindSpeed = 35,
};

var options = new JsonSerializerOptions
{
	PropertyNamingPolicy = new UpperCaseNamingPolicy(),
	WriteIndented = true
};
var jsonString = JsonSerializer.Serialize(weatherForecast, options);

jsonString.Dump();


public class UpperCaseNamingPolicy : JsonNamingPolicy
{
	public override string ConvertName(string name) =>
		name.ToUpper();
}

public class WeatherForecastWithPropertyNameAttribute
{
	public DateTimeOffset Date { get; set; }
	public int TemperatureCelsius { get; set; }
	public string Summary { get; set; }
	[JsonPropertyName("Wind")]
	public int WindSpeed { get; set; }
}