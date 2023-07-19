<Query Kind="Program">
  <NuGetReference Version="3.0.1">Serilog</NuGetReference>
  <NuGetReference>Serilog.Sinks.Console</NuGetReference>
  <Namespace>Serilog</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	//serilog是一种序列化器。
	Log.Logger = new LoggerConfiguration()
			.WriteTo.Console(outputTemplate : "[{Level}] {Timestamp:yyyy-MM-dd HH:mm:ss.fff}  {Message:lj}{NewLine}{Exception}")
			.CreateLogger();
	
	
	//1.Default Behaviour 
	//1.1 Simple, Scalar Values
	var count = 456;
	Log.Information("Retrieved {Count} records", count);
	
	//1.2 Collections IEnumerable
	var fruit = new[] { "Apple", "Pear", "Orange" };
	Log.Information("In my bowl I have {Fruit}", fruit);
	
	//1.3 Collections Dictionary
	var fruitDic = new Dictionary<string, int> { { "Apple", 1 }, { "Pear", 5 } };
	Log.Information("In my bowl I have {Fruit}", fruitDic);

	//1.4 Objects 如果不识别对象 则用toString输出
	//自定义类的对象
	Teacher teacher = new Teacher("tim", 32);
	Student student = new Student("ge", 23);
	student.teacher = teacher;
	Log.Information("teacher {Teacher} Student {student}", teacher,student);
	
	HttpClient httpClient = new HttpClient();
	Log.Information("HttpClient {httpClient}", httpClient);

	//2.1 Preserving Object Structure 保存对象结构
	//使用@符号 可以保存成key value的形式
	Log.Information("teacher {@Teacher} Student {@student}", teacher, student);
	Log.Information("HttpClient {@httpClient}", httpClient);

	//2.2 Customizing the stored data
	Log.Logger = new LoggerConfiguration()
			.Destructure.ByTransforming<Student>(s => new { Name = s.Name, Age = s.Age })
			.WriteTo.Console(outputTemplate: "[{Level}] {Timestamp:yyyy-MM-dd HH:mm:ss.fff}  {Message:lj}{NewLine}{Exception}")
			.CreateLogger();
	Log.Information("teacher {@Teacher} Student {@student}", teacher,student);
	
	//3.Forcing Stringification 强制字符串输出
	var unknown = new [] { 1, 2, 3 };
	Log.Information("Received {$Data}", unknown);
}

// You can define other methods, fields, classes and namespaces here

abstract class Person
{
	public String Name { get; set; }

	public int Age { get; set; }
}
class Teacher : Person
{
	private Guid GuidID { get; set; }
	public Teacher( string name, int age)
	{
		this.GuidID = Guid.NewGuid();
		this.Name = name;
		this.Age = age;
	}
}
class Student : Person
{
	public Student(string name, int age)
	{
		this.GuidID = Guid.NewGuid();
		this.Name = name;
		this.Age = age;
	}
	private Guid GuidID { get; set; }
	public Teacher teacher { get; set; }
}