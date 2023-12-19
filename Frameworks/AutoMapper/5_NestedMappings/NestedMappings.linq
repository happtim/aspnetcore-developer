<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

// Nested Mapptings (嵌套映射)
// AutoMapper 执行映射的过程中，支持嵌套的映射。成员变量类型也在AutoMappter配置中，也会对该成员变量进行映射。


var config = new MapperConfiguration(cfg =>
{
	cfg.CreateMap<OuterSource, OuterDest>();
	cfg.CreateMap<InnerSource, InnerDest>();
});
config.AssertConfigurationIsValid();
var mapper = config.CreateMapper();

var source = new OuterSource
{
	Value = 5,
	Inner = new InnerSource { OtherValue = 15 }
};

var dest = mapper.Map<OuterSource, OuterDest>(source);

dest.Dump();



public class OuterSource
{
	public int Value { get; set; }
	public InnerSource Inner { get; set; }
}

public class InnerSource
{
	public int OtherValue { get; set; }
}

public class OuterDest
{
	public int Value { get; set; }
	public InnerDest Inner { get; set; }
}

public class InnerDest
{
	public int OtherValue { get; set; }
}