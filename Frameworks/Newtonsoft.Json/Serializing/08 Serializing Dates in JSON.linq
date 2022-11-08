<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
</Query>

//Json.NET 使用的默认格式是ISO 8601标准：“2012-03-19T07:22Z”。

var entry = new 
{
	LogDate = new DateTime(2009, 2, 15, 0, 0, 0, DateTimeKind.Utc),
	Details = "Application started."
};

// default as of Json.NET 4.5
string isoJson = JsonConvert.SerializeObject(entry);
isoJson.Dump();

//IsoDateTimeConverter 自定义format
isoJson = JsonConvert.SerializeObject(entry, new IsoDateTimeConverter() {DateTimeFormat = "yyyy-MM-dd HH:mm:ss"});
isoJson.Dump();

//DateFormatHandling
JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
{
	DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
};
string microsoftJson = JsonConvert.SerializeObject(entry, microsoftDateFormatSettings);
microsoftJson.Dump();

//JavaScriptDateTimeConverter 此转换器序列化 作为 JavaScript 的日期时间日期对象：new Date（1234656000000）
string javascriptJson = JsonConvert.SerializeObject(entry, new JavaScriptDateTimeConverter());
javascriptJson.Dump();

