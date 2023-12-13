<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>


var foo = new Foo() { Name = "Foo" };

//Naming Conventions （命名约定）
//下面配置这会将以下属性相互映射： property_name -> PropertyName
//如果不需要命名约定，可以使用。ExactMatchNamingConvention
//命名约定就三个类LowerUnderscoreNamingConvention，PascalCaseNamingConvention，ExactMatchNamingConvention
var config = new MapperConfiguration(cfg =>
{
		// 直接设置配置。
	cfg.CreateMap<Foo, FooDto>();
	cfg.SourceMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
	cfg.DestinationMemberNamingConvention = PascalCaseNamingConvention.Instance;
});

foo.property_name = "命名约定";
var mapper = config.CreateMapper();
mapper.Map<FooDto>(foo).Dump();


class Foo
{
	public string Name { get; set; }
	public string property_name { get; set; }

}

class FooDto : Foo
{
	public string PropertyName {get;set;}
}