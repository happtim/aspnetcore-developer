<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

// Construction （构造函数）

// 如果目标构造函数参数名称不匹配，可以在配置时修改它们：
var configuration = new MapperConfiguration(cfg =>
  cfg.CreateMap<Source, SourceDto>()
	//没有该代码 找构造函数失败，报错
	.ForCtorParam("valueParamSomeOtherName", opt => opt.MapFrom(src => src.Value))
);
var mapper = configuration.CreateMapper();
mapper.Map<SourceDto>(new Source() { Value = 10}).Dump();


public class Source
{
	public int Value { get; set; }
}

public class SourceDto
{
	public SourceDto(int valueParamSomeOtherName)
	{
		_value = valueParamSomeOtherName;
	}
	private int _value;
	public int Value
	{
		get { return _value; }
	}
}
