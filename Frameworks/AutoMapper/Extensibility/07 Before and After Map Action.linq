<Query Kind="Program">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

// Before and After Map Action (事件)

//有时，您可能需要在映射发生之前或之后执行自定义逻辑。这个工作很罕见。可以在映射之外做这些事情。

void Main()
{
	var configuration = new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<Source, Dest>()
		  .BeforeMap((src, dest) => src.Value = src.Value + 10)
		  .AfterMap((src, dest) => dest.Name = "John");
	});
	
	var mapper = configuration.CreateMapper();
	Source source = new Source{Name ="Tim", Value = 10};
	mapper.Map<Dest>(source).Dump();
}

public class Source
{
	public string Name { get; set; }
	public int Value {get;set;}
}

public class Dest
{
	public string Name { get; set; }
	public int Value {get;set;}
}