<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//MapperConfiguration 创建一个实例并通过构造函数初始化配置
//MapperConfiguration 实例可以静态存储在静态字段或依赖项注入容器中。一旦创建，就无法更改/修改。
var foo = new Foo() { Name = "Foo" };
var fooDto = new FooDto { Value = "20"};

var config = new MapperConfiguration(cfg =>
{
		// 直接设置配置。
	cfg.CreateMap<Foo, FooDto>();
});

var mapper = config.CreateMapper();
//映射创建新的对象
mapper.Map<FooDto>(foo).Dump();
//映射已有的对象
mapper.Map(foo,fooDto).Dump();

class Foo
{
	public string Name { get; set; }
}

class FooDto : Foo
{
	public string Value {get; set; }
}