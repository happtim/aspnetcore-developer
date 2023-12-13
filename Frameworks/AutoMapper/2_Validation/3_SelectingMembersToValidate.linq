<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>


//默认使用目标的Member来映射约定，
//可以使用 MemberList.Source 或者 MemberList.None 来修改映射方向

var config = new MapperConfiguration(cfg =>
{
	cfg.CreateMap<Source, Destination>(MemberList.None);
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