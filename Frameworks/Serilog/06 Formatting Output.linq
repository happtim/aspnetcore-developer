<Query Kind="Program">
  <NuGetReference>Serilog</NuGetReference>
  <NuGetReference>Serilog.Formatting.Compact</NuGetReference>
  <NuGetReference>Serilog.Sinks.Console</NuGetReference>
  <Namespace>Serilog</Namespace>
  <Namespace>Serilog.Formatting.Compact</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	//Formatting plain text
	
	// Sinks 通过 outputTemplate 输出模板 来控制如何纯文本输出。
	Log.Logger = new LoggerConfiguration()
		.WriteTo.Console(outputTemplate:
			"[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
		.CreateLogger();
	
	//内置property
	// * Exception 跨多行格式化的完整异常消息和堆栈跟踪。如果没有异常与事件关联，则为空。
	// * Level  日志事件级别，格式化为完整级别名称。:u3 三个字符紧凑型。:w3 三字符小写名称
	// * Message 日志事件的消息，呈现为纯文本。:l 引号没有，:j json风格。
	// * NewLine
	// * Properties 未显示在输出中其他位置的所有事件属性值。使用格式以使用 JSON 呈现。:j
	// * Timestamp  件的时间戳，作为 .DateTimeOffset
	
	//Formatting JSON 格式化Json
	//Install-Package Serilog.Formatting.Compact 
	// 1. Serilog.Formatting.Json.JsonFormatter 这是 Serilog 软件包中附带的历史默认值
	// 2. Serilog.Formatting.Compact.CompactJsonFormatter 一个更新，更节省空间的JSON格式化程序
	Log.Logger = new LoggerConfiguration()
		.WriteTo.Console(new CompactJsonFormatter())
		.CreateLogger();
	string user = "timge";
	Log.Information("Hello, {User}",user);
	
	//Format providers
	//有许多选项可用于格式化各个类型（如日期）的输出。
	
	Log.Logger = new LoggerConfiguration()
		.WriteTo.Console()
		.CreateLogger();
	
	var exampleUser = new User { Id = 1, Name = "Adam", Created = DateTime.Now };
	Log.Information("Created {@User} on {Created}", exampleUser, DateTime.Now);


	var formatter = new CustomDateFormatter("yyyy-MMM-dd", new CultureInfo("zh-cn"));
	Log.Logger = new LoggerConfiguration()
		.WriteTo.Console(formatProvider: new CultureInfo("zh-cn")) // Console 1
		.WriteTo.Console(formatProvider: formatter)                // Console 2
		.CreateLogger();

	exampleUser = new User { Id = 1, Name = "Adam", Created = DateTime.Now };
	Log.Information("Created {@User} on {Created}", exampleUser, DateTime.Now);

}

class User
{
	public int Id { get; set; }
	public string Name { get; set; }
	public DateTime Created { get; set; }
}

class CustomDateFormatter : IFormatProvider
{
	readonly IFormatProvider basedOn;
	readonly string shortDatePattern;
	public CustomDateFormatter(string shortDatePattern, IFormatProvider basedOn)
	{
		this.shortDatePattern = shortDatePattern;
		this.basedOn = basedOn;
	}
	public object GetFormat(Type formatType)
	{
		if (formatType == typeof(DateTimeFormatInfo))
		{
			var basedOnFormatInfo = (DateTimeFormatInfo)basedOn.GetFormat(formatType);
			var dateFormatInfo = (DateTimeFormatInfo)basedOnFormatInfo.Clone();
			dateFormatInfo.ShortDatePattern = this.shortDatePattern;
			return dateFormatInfo;
		}
		return this.basedOn.GetFormat(formatType);
	}
}
