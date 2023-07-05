<Query Kind="Program">
  <NuGetReference Version="3.0.1">Serilog</NuGetReference>
  <NuGetReference Version="4.1.0">Serilog.Sinks.Console</NuGetReference>
  <Namespace>Serilog</Namespace>
  <Namespace>Serilog.Core</Namespace>
  <Namespace>Serilog.Events</Namespace>
</Query>

void Main()
{
	Log.Logger = new LoggerConfiguration()
			.WriteTo.Console()
			.CreateLogger();
	int quota = 1024;
	string user = "tim";
	Log.Warning("Disk quota {Quota} MB exceeded by {User}", quota, user);
	
	//Message Template Syntax
	// "Disk quota {Quota} MB exceeded by {User}" is message template。并且是string.format 的超集，所以format函数功能它都可以使用。
	
	// *属性（Property）需要在{}中
	// *属性（Property）命名要符合C#的变量命令
	// *属性 也是用使用{0} {1} 数字占位符。
	Log.Warning("Disk quota {0} MB exceeded by {1}", quota, user);
	// *属性 如果出现一个非数字的，那么就按照从左到右顺序依次对应参数。
	Log.Warning("Disk quota {Quota} MB exceeded by {0}", quota, user);
	// *属性 有两个操作符 @ （结构），$（强制字符串）
	// *属性 可以有后缀 如“:000”,用来控制属性的输出格式。
	
	//Message Template Recommendations
	// 1. Fluent Style Guideline 良好的 Serilog 事件使用属性的名称作为消息中的内容，这提高了可读性并使事件更加紧凑。
	// 2. 日志事件消息是片段，而不是句子。
	// 3. 记录日志使用Message Template （消息模板），而不是Message（消息）。Serilog解析存储每个Message Template，
	// 如果将每个log内容都要处理成消息模板，将会降低日志记录的性能。
	
	// Don't:
	Log.Information("The time is " + DateTime.Now);
	
	// Do:
	Log.Information("The time is {Now}", DateTime.Now);

	//Dynamic levels
	//有些大型的软件默认在Info或者warin级别运行，他们会在出现异常，收集到更多的数据后，才打开更调式级别的日志。
	
	var levelSwitch = new LoggingLevelSwitch();
	levelSwitch.MinimumLevel = LogEventLevel.Warning;

	var log = new LoggerConfiguration()
		.MinimumLevel.ControlledBy(levelSwitch)
		.WriteTo.Console()
		.CreateLogger();

	levelSwitch.MinimumLevel = LogEventLevel.Verbose;
	log.Verbose("This will now be logged");

	//Source Contexts 
	// Serilog 允许使用其源标记事件
	// myLog 使用ForContext 函数之后 是的自身具有一个 “SourceContext” 属性。我们可以对这个属性进行筛选（filter），选择进入某些sinks （sub-logs）。
	Log.Logger = new LoggerConfiguration()
		.WriteTo.Console(outputTemplate: "[{Level}] {Timestamp:yyyy-MM-dd HH:mm:ss.fff} {SourceContext} {Message:lj}{NewLine}{Exception}")
		.CreateLogger();
	var myLog = Log.ForContext<Job>();
	myLog.Information("Hello! Job Full Name {FullName}", typeof(Job).FullName);

	//Soucee Contexts way two
	//和上面例子类似，其他重载使日志事件能够使用标识符进行标记
	Log.Logger = new LoggerConfiguration()
	.WriteTo.Console(outputTemplate: "[{Level}] {Timestamp:yyyy-MM-dd HH:mm:ss.fff} {JobId} {Message:lj}{NewLine}{Exception}")
	.CreateLogger();
	
	var job = new Job{Id= 1};
	var jobLog = Log.ForContext("JobId", job.Id);
	jobLog.Information("Running a new job");
	job.Run();
	jobLog.Information("Finished");
}

class Job{
	public int Id {get;set;}
	
	public void Run(){
		Console.WriteLine("doing somethings.");
	}
}
