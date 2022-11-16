<Query Kind="Program">
  <NuGetReference>AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//Custom Type Converters （自定义类型转换器）

//有时，您需要完全控制一种类型到另一种类型的转换。这通常是当一种类型看起来与另一种类型完全不同。

// 假设我们有源类型和目标类型。如果直接进行转换AutoMapper会抛出异常。应为AutoMapper不知道如何将string->int.或者时间，Type。
// 为了能进行转化我们需要自己创建 type converter。

//void ConvertUsing(Func<TSource, TDestination> mappingFunction); 只是任何接受源并返回目标的函数
//void ConvertUsing(ITypeConverter<TSource, TDestination> converter); 在更困难的情况下，我们可以创建自定义的ITypeConverter<Startination>：
//void ConvertUsing<TTypeConverter>() where TTypeConverter : ITypeConverter<TSource, TDestination>;

public class Source
{
	public string Value1 { get; set; }
	public string Value2 { get; set; }
	public string Value3 { get; set; }
}

public class Destination
{
	public int Value1 { get; set; }
	public DateTime Value2 { get; set; }
	public Type Value3 { get; set; }
}


void Main()
{
	var configuration = new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<string, int>().ConvertUsing(s => Convert.ToInt32(s));
		cfg.CreateMap<string, DateTime>().ConvertUsing(new DateTimeTypeConverter());
		cfg.CreateMap<string, Type>().ConvertUsing<TypeTypeConverter>();
		cfg.CreateMap<Source, Destination>();
	});
	
	configuration.AssertConfigurationIsValid();

	var mapper = configuration.CreateMapper();
	
	var source = new Source
	{
		Value1 = "5",
		Value2 = "01/01/2000",
		Value3 = "UserQuery+Destination"
	};

	Destination result = mapper.Map<Source, Destination>(source);
	result.Dump();
}


public class DateTimeTypeConverter : ITypeConverter<string, DateTime>
{
	public DateTime Convert(string source, DateTime destination, ResolutionContext context)
	{
		return System.Convert.ToDateTime(source);
	}
}

public class TypeTypeConverter : ITypeConverter<string, Type>
{
	public Type Convert(string source, Type destination, ResolutionContext context)
	{
		return Assembly.GetExecutingAssembly().GetType(source);
	}
}


