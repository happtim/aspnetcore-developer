<Query Kind="Program">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//Conditional Mapping （条件映射）

//AutoMapper允许您向属性添加条件，这些条件在映射该属性之前必须满足这些条件。
void Main()
{
	var configuration = new MapperConfiguration(cfg =>
	{
		cfg.CreateMap<Foo, Bar>()
		  .ForMember(dest => dest.baz, opt => {
		  	//opt.Condition(src => (src.baz >= 0)); //automapper 内部异常
			opt.PreCondition(src => (src.baz >= 0));
		  }
		);
	});
	
	var mapper = configuration.CreateMapper();
	Foo foo = new Foo{baz = 10};
	mapper.Map<Bar>(foo).Dump();
	foo.baz = -100;
	mapper.Map<Bar>(foo).Dump();
}

class Foo
{
	public int baz;
}

class Bar
{
	private uint _baz;
	public uint baz
	{
		get {return _baz;}
		set
		{
			_baz = value;
		}
	}
}