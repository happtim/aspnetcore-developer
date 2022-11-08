<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Runtime.Serialization</Namespace>
</Query>

void Main()
{
	// Serialization Error Handling （序列化错误处理）
	// Json.Net 支持序列化和反序列化的错误处理，错误处理可以catch一个异常处理他，或者让他往上层调用抛。
	
	// Error Event （错误事件）
	// Error event 是一个 JsonSerializer 上的一个时间处理函数程序，当发生异常时，就会调用错误事件。
	// 和其他JsonSerializer的配置一样， 他也可以在sonSerializerSettings上设置。
	
	List<string> errors = new List<string>();
	
	// Error Event 事件中，捕获的事件标记为已处理，就不会向上抛出异常。
	List<DateTime> c = JsonConvert.DeserializeObject<List<DateTime>>(@"[
	      '2009-09-09T00:00:00Z',
	      'I am not a date and will error!',
	      [
	        1
	      ],
	      '1977-02-20T00:00:00Z',
	      null,
	      '2000-12-01T00:00:00Z'
	    ]",
		new JsonSerializerSettings
		{
			Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
			{
				errors.Add(args.ErrorContext.Error.Message);
				args.ErrorContext.Handled = true;
			},
			Converters = { new IsoDateTimeConverter() }
		});
		
	errors.Dump();
	
	
	errors.Clear();
	//Json.NET 中的错误处理需要注意的一件事，是未处理的错误将冒泡并在其每个父节点上引发事件。
	//如下例子：序列化对象集合时出现未处理的错误 将引发两次，一次针对对象，然后再次针对集合。 这将允许您处理发生错误的在两个位置。
	//如果只想处理一次。可以使用ErrorEventArgs 的 CurrentObject 等于 OriginalObject。OriginalObject 是引发错误的对象，CurrentObject 是 引发事件所针对的对象。
	try
	{
		c = JsonConvert.DeserializeObject<List<DateTime>>(@"[
	      '2009-09-09T00:00:00Z',
	      'I am not a date and will error!',
	      [
	        1
	      ],
	      '1977-02-20T00:00:00Z',
	      null,
	      '2000-12-01T00:00:00Z'
	    ]",
		new JsonSerializerSettings
		{
			Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
			{
				// only log an error once
				if (args.CurrentObject == args.ErrorContext.OriginalObject)
				{
					errors.Add(args.ErrorContext.Error.Message);
				}
			},
			Converters = { new IsoDateTimeConverter() }
		});
	}
	catch (Exception ex)
	{
		ex.Dump();
	}
	
	errors.Dump();

	// OnErrorAttribute
	// 也可以通过OnError属性处理序列化的异常。他有两个参数StreamingContext 和 ErrorContext. 
	PersonError person = new PersonError
	{
		Name = "George Michael Bluth",
		Age = 16,
		Roles = null,
		Title = "Mister Manager"
	};

	string json = JsonConvert.SerializeObject(person, Newtonsoft.Json.Formatting.Indented);

	person.Dump();
	json.Dump();
}


public class PersonError
{
	private List<string> _roles;
	private List<string> _errors = new List<string>(); 

	public string Name { get; set; }
	public int Age { get; set; }

	public List<string> Roles
	{
		get
		{
			if (_roles == null)
			{
				throw new Exception("Roles not loaded!");
			}

			return _roles;
		}
		set { _roles = value; }
	}

	public string Title { get; set; }

	[OnError]
	internal void OnError(StreamingContext context, ErrorContext errorContext)
	{
		errorContext.Handled = true;
		this._errors.Add(errorContext.Error.Message);
	}
}
