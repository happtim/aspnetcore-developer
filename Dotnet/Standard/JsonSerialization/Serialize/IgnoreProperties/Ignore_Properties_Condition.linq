<Query Kind="Statements">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

//可以通过设置[JsonIgnore] 特性的 Condition 属性来指定条件排除。 JsonIgnoreCondition 枚举提供下列选项：
//
//Always - 始终忽略属性。 如果未指定 Condition，则假设此选项。
//Never - 无论 DefaultIgnoreCondition、IgnoreReadOnlyProperties 和 IgnoreReadOnlyFields 全局设置如何，始终序列化和反序列化属性。
//WhenWritingDefault - 如果属性是引用类型 null可为 null 的值类型 null 或值类型 default，则在序列化中忽略属性。
//WhenWritingNull - 如果属性是引用类型 null 或可为 null 的值类型 null，则在序列化中忽略属性。

var weatherForecast = new WeatherForecastWithIgnoreCondition
{						 
	Date = default,
    Summary = null,
    TemperatureC = default

};

var options = new JsonSerializerOptions { WriteIndented = true };
string jsonString = JsonSerializer.Serialize(weatherForecast, options);

jsonString.Dump();

public class WeatherForecastWithIgnoreCondition
{
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public DateTime Date { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.Never)]
	public int TemperatureC { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Summary { get; set; }
};