<Query Kind="Program">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

// Value Transformers （值转化器）

// Value Transformers 可以添加额外的转化给一个类型。
// 它可以多个级别应用。

//Globally
//Profile
//Map
//Member

void Main()
{
	var configuration = new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<Source,Dest>();
		cfg.ValueTransformers.Add<string>(val => val + "!!!");
	});

	var mapper = configuration.CreateMapper();
	var source = new Source { Value = "Hello" };
	var dest = mapper.Map<Dest>(source);
	dest.Dump();

	 configuration = new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<Source, Dest>()
			.ValueTransformers.Add<string>(val => val + "!!!")
			;
	});
	mapper = configuration.CreateMapper();
	dest = mapper.Map<Dest>(source);
	dest.Dump();

	configuration = new MapperConfiguration(cfg =>
	{
	   cfg.CreateMap<Source, Dest>()
	   		.ForMember(dest => dest.Value, opt => opt.AddTransform(val => val + "!!!") )
		   ;
	});
	mapper = configuration.CreateMapper();
	dest = mapper.Map<Dest>(source);
	dest.Dump();
}

public class Source
{
	public string Value {get;set;}
}

public class Dest
{
	public string Value {get;set;}
}

