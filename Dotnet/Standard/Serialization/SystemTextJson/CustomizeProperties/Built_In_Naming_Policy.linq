<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

//CamelCase 第一个单词以小写字符开头。连续单词以大写字符开头。TempCelsius	tempCelsius

//8.0 新增
//KebabCaseLower * 单词由连字符分隔。所有字符均为小写。	TempCelsius temp-celsius
//KebabCaseUpper * 单词由连字符分隔。所有字符均为大写。	TempCelsius TEMP-CELSIUS
//SnakeCaseLower * 单词用下划线分隔。所有字符均为小写。	TempCelsius temp_celsius
//SnakeCaseUpper * 单词用下划线分隔。所有字符均为大写。	TempCelsius TEMP_CELSIUS

// * 适用于序列化和反序列化。
// * 由[JsonPropertyName] 特性替代。 这便是示例中的 JSON 属性名称 Wind 不是 camel 大小写的原因。
var weatherForecast = new WeatherForecastWithPropertyNameAttribute
{
	Date = DateTime.Now,
	TemperatureCelsius = 25,
	Summary = "Hot",
	WindSpeed = 35,
};

var options = new JsonSerializerOptions
{
	PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
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