<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>


//使用AssertConfigurationIsValid 方法检查配置项
//发现 Name， SomeValuefff 都没有对应映射。所以报错。

var config = new MapperConfiguration(cfg =>
{
	cfg.CreateMap<Source, Destination>();
});

config.AssertConfigurationIsValid();

var mapper = config.CreateMapper();
mapper.Map<Destination>(new Source {SomeValue = 1}).Dump();


public class Source
{
	public int SomeValue { get; set; }
}

public class Destination
{
	public string Name {get;set;}
	public int SomeValuefff  { get; set; }
}