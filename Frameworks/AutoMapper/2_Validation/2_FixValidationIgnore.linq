<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>


// 可以使用三种方法解决
//1. Custom Value Resolvers
//2. Projection
//3. Ignore() 

//该案例使用Ingore
var config = new MapperConfiguration(cfg =>
{
	cfg.CreateMap<Source, Destination>()
		.ForMember(dest => dest.SomeValuefff, opt => opt.Ignore() )
		.ForMember(dest => dest.Name, opt => opt.Ignore() );
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