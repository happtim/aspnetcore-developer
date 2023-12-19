<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//Configuring visibility

//默认情况下，AutoMapper只识别公共成员。它甚至可以映射到私有的setter
//但如果整个属性是私有的/内部的，则会跳过内部/私有的方法和属性。
//要指示AutoMapper识别具有其他可见性的成员，请覆盖默认过滤器ShouldMapField和/或ShouldMapProperty。
var foo = new Foo() { Name = "Foo" };

var config = new MapperConfiguration(cfg =>
{
	// 直接设置配置。
	cfg.CreateMap<Foo, FooDto>();
	// map properties with public or internal getters
	cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
});

var mapper = config.CreateMapper();
//映射创建新的对象
mapper.Map<FooDto>(foo).Dump();

class Foo
{
	public string Name { get; set; }
}

class FooDto
{
	public string Name { get; private set; }
}