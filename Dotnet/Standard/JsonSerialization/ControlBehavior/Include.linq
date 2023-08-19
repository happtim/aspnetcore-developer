<Query Kind="Statements">
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Text.Json</Namespace>
</Query>



var json = @"{""Date"":""2020-09-06T11:31:01.923395"",""TemperatureC"":-1,""Summary"":""Cold""} ";
Console.WriteLine($"Input JSON: {json}");

var options = new JsonSerializerOptions
{
	IncludeFields = true,
};
var forecast = JsonSerializer.Deserialize<Forecast>(json, options)!;

forecast.Dump();

var roundTrippedJson = JsonSerializer.Serialize<Forecast>(forecast, options);

Console.WriteLine($"Output JSON: {roundTrippedJson}");

var forecast2 = JsonSerializer.Deserialize<Forecast2>(json)!;

forecast2.Dump();

roundTrippedJson = JsonSerializer.Serialize<Forecast2>(forecast2);

Console.WriteLine($"Output JSON: {roundTrippedJson}");

public class Forecast
{
	public DateTime Date;
	public int TemperatureC;
	public string? Summary;
}

public class Forecast2
{
	[JsonInclude]
	public DateTime Date;
	[JsonInclude]
	public int TemperatureC;
	[JsonInclude]
	public string? Summary;
}