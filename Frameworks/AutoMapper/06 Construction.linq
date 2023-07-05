<Query Kind="Program">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

// Construction （构造函数）

void Main()
{
	//autoMapper 可以根据源类型成员 映射成目标类型的构造函数。
	var configuration = new MapperConfiguration(cfg => cfg.CreateMap<Source, SourceDto>());
	var mapper =  configuration.CreateMapper();

	mapper.Map<SourceDto>(new Source() { Value = 10}).Dump();

	// 如果目标构造函数参数名称不匹配，可以在配置时修改它们：
	 configuration = new MapperConfiguration(cfg =>
	  cfg.CreateMap<Source, SourceDto2>()
		.ForCtorParam("valueParamSomeOtherName", opt => opt.MapFrom(src => src.Value))
	);
	mapper = configuration.CreateMapper();
	mapper.Map<SourceDto2>(new Source() { Value = 10}).Dump();

}


public class Source
{
	public int Value { get; set; }
}
public class SourceDto
{
	public SourceDto(int value)
	{
		_value = value;
	}
	private int _value;
	public int Value
	{
		get { return _value; }
	}
}

public class SourceDto2
{
	public SourceDto2(int valueParamSomeOtherName)
	{
		_value = valueParamSomeOtherName;
	}
	private int _value;
	public int Value
	{
		get { return _value; }
	}
}
