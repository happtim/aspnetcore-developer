<Query Kind="Program">
  <NuGetReference>AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

// Custom Value Resolvers （自定义值解析程序）

// 有一部分映射情况是，目标类型值，需要一些转化得到。
// 在这种情况下就需要 custom value resolvers （自定义值解析）方法

//接口定义如下：
//public interface IValueResolver<in TSource, in TDestination, TDestMember>
//{
//	TDestMember Resolve(TSource source, TDestination destination, TDestMember destMember, ResolutionContext context);
//}

//配置方式如下：
//MapFrom<TValueResolver>
//MapFrom(typeof(CustomValueResolver))
//MapFrom(aValueResolverInstance)

void Main()
{
	// 我们有Source，Destination两个类，在Source中我们没有方法进行Value1，2的相加。

	var configuration = new MapperConfiguration(cfg =>
 		  cfg.CreateMap<Source, Destination>()
			 .ForMember(dest => dest.Total, opt => opt.MapFrom<CustomResolver>())
	);
	configuration.AssertConfigurationIsValid();

	var mapper = configuration.CreateMapper();
	var source = new Source
	{
		Value1 = 5,
		Value2 = 7
	};

	var result = mapper.Map<Source, Destination>(source);
	result.Dump();
	
	//Custom constructor methods （自定义构造函数）
	// 
}

public class Source
{
	public int Value1 { get; set; }
	public int Value2 { get; set; }
}

public class Destination
{
	public int Total { get; set; }
}

public class CustomResolver : IValueResolver<Source, Destination, int>
{
	public int Resolve(Source source, Destination destination, int member, ResolutionContext context)
	{
		return source.Value1 + source.Value2;
	}
}
