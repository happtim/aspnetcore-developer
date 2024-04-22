<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <NuGetReference>BenchmarkDotNet</NuGetReference>
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
  <Namespace>BenchmarkDotNet.Running</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>AutoMapper</Namespace>
</Query>

//需要把People字段值映射到PeopleCopy字段
#LINQPad optimize+     // Enable compiler optimizations

var summary = BenchmarkRunner.Run<ReflectionMapperBenchmark>();


[ShortRunJob]
public class ReflectionMapperBenchmark
{
	private People people = new People
	{
		Id = 5,
		Age = 12,
		Name = "张三",
	};
	
	private IMapper mapper;
	
	public ReflectionMapperBenchmark()
	{

		var config = new MapperConfiguration(cfg =>
		{
			// 直接设置配置。
			cfg.CreateMap<People, PeopleCopy>();
		});
		
		mapper = config.CreateMapper();
	}

	[Benchmark]
	public void SerializeMapper_Trans()
	{
		string strJson = JsonSerializer.Serialize(people);
		JsonSerializer.Deserialize<PeopleCopy>(strJson);
	}
	
	[Benchmark]
	public void ReflectionMapper_Trans()
	{
		var result = ReflectionMapper.Trans<People, PeopleCopy>(people);
	}


	[Benchmark]
	public void ExpressionMapper_Trans()
	{
		 PeopleCopy peopleCopy = ExpressionMapper.Trans<People, PeopleCopy>(people);
	}


	[Benchmark]
	public void AutoMapper_Trans()
	{
		mapper.Map<PeopleCopy>(people);
	}

	[Benchmark]
	public void ExpressionGenericMapper_Trans()
	{
		PeopleCopy peopleCopy = ExpressionGenericMapper<People, PeopleCopy>.Trans(people);
	}

	[Benchmark]
	public void NewPeopleCopy()
	{
		PeopleCopy peopleCopy = new PeopleCopy()
		{
			Id = people.Id,
			Name = people.Name,
			Age = people.Age
		};
	}
}

public class ReflectionMapper
{
	/// <summary>
	/// 反射
	/// </summary>
	/// <typeparam name="TIn"></typeparam>
	/// <typeparam name="TOut"></typeparam>
	/// <param name="tIn"></param>
	/// <returns></returns>
	public static TOut Trans<TIn, TOut>(TIn tIn)
	{
		TOut tOut = Activator.CreateInstance<TOut>();
		foreach (var itemOut in tOut.GetType().GetProperties())
		{
			var propIn = tIn.GetType().GetProperty(itemOut.Name);
			itemOut.SetValue(tOut, propIn.GetValue(tIn));
		}

		foreach (var itemOut in tOut.GetType().GetFields())
		{
			var fieldIn = tIn.GetType().GetField(itemOut.Name);
			itemOut.SetValue(tOut, fieldIn.GetValue(tIn));
		}
		return tOut;
	}
}

public class ExpressionMapper
{
	/// <summary>
	/// 字典缓存，保存的是委托，委托内部是转换的动作
	/// </summary>
	private static Dictionary<string, object> _Dic = new Dictionary<string, object>();

	/// <summary>
	/// Expression动态拼接+普通缓存
	/// </summary>
	/// <typeparam name="TIn"></typeparam>
	/// <typeparam name="TOut"></typeparam>
	/// <param name="tIn"></param>
	/// <returns></returns>
	public static TOut Trans<TIn, TOut>(TIn tIn)
	{
		string key = $"funckey_{typeof(TIn).FullName}_{typeof(TOut).FullName}";
		if (!_Dic.ContainsKey(key))
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
			List<MemberBinding> memberBindingList = new List<MemberBinding>();
			foreach (var item in typeof(TOut).GetProperties())
			{
				MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
				MemberBinding memberBinding = Expression.Bind(item, property);
				memberBindingList.Add(memberBinding);
			}
			foreach (var item in typeof(TOut).GetFields())
			{
				MemberExpression property = Expression.Field(parameterExpression, typeof(TIn).GetField(item.Name));
				MemberBinding memberBinding = Expression.Bind(item, property);
				memberBindingList.Add(memberBinding);
			}
			MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
			Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[]
			{
					parameterExpression
			});
			Func<TIn, TOut> func = lambda.Compile();//拼装是一次性的
			_Dic[key] = func;
		}
		return ((Func<TIn, TOut>)_Dic[key]).Invoke(tIn);
	}
}

/// <summary>
/// Expression动态拼接+泛型缓存
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public class ExpressionGenericMapper<TIn, TOut>//Mapper`2
{
	private static Func<TIn, TOut> _FUNC = null;
	static ExpressionGenericMapper()
	{
		ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
		List<MemberBinding> memberBindingList = new List<MemberBinding>();
		foreach (var item in typeof(TOut).GetProperties())
		{
			MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
			MemberBinding memberBinding = Expression.Bind(item, property);
			memberBindingList.Add(memberBinding);
		}
		foreach (var item in typeof(TOut).GetFields())
		{
			MemberExpression property = Expression.Field(parameterExpression, typeof(TIn).GetField(item.Name));
			MemberBinding memberBinding = Expression.Bind(item, property);
			memberBindingList.Add(memberBinding);
		}
		MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
		Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[]
		{
					parameterExpression
		});
		_FUNC = lambda.Compile();//拼装是一次性的
	}
	public static TOut Trans(TIn t)
	{
		return _FUNC(t);
	}
}

class People
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int Age { get; set; }
}

class PeopleCopy : People{}