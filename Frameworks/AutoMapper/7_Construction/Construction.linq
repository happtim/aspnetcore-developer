<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

// Construction （构造函数）

//autoMapper 可以根据源类型成员 映射成目标类型的构造函数。

var configuration = new MapperConfiguration(cfg => 
	{ 
		cfg.CreateMap<Source, SourceDto>();
		
		//构造函数的开放默认是任意，也可以配置指定开发类型
		//cfg.ShouldUseConstructor = (constructor) => {  
		//	return constructor.IsPublic || constructor.IsPrivate;
		//};
		
		//禁用构造函数
		//cfg.DisableConstructorMapping();
	});
var mapper = configuration.CreateMapper();

mapper.Map<SourceDto>(new Source() { Value = 10}).Dump();


public class Source
{
	public int Value { get; set; }
}
public class SourceDto
{
	private SourceDto(int value)
	{
		Console.WriteLine("SourceDto(int)");
		_value = value;
	}
	private int _value;
	public int Value
	{
		get { return _value; }
	}
}
