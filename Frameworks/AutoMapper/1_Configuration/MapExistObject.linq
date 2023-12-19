<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//Mapping Existing Object

//Map 有方法可以映射一个已有的对象
var foo = new Foo() { Name = "Foo" };
var fooDto = new FooDto { Value = 20 };

var config = new MapperConfiguration(cfg =>
{
	// 直接设置配置。
	cfg.CreateMap<Foo, FooDto>();
});

var mapper = config.CreateMapper();

//映射已有的对象
mapper.Map(foo, fooDto).Dump();

class Foo
{
	public string Name { get; set; }
}

class FooDto
{
	public string Name { get; set; }
	public int Value { get; set; }
}