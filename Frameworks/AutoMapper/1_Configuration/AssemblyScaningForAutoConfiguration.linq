<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>


var foo = new Foo() { Name = "Foo" };

//Assembly Scanning for auto configuration （用于自动配置的装配扫描）
//AutoMapper 将扫描指定的程序集以查找从配置文件继承的类，并将其添加到配置中。
var myAssembly = Assembly.GetExecutingAssembly();
var config = new MapperConfiguration(cfg =>
{
		//扫描程序集的方式配置
	cfg.AddMaps(myAssembly);

		//使用assembly name的方式配置
	cfg.AddMaps(new[] {
			"LINQPadQuery"
		}
	);

		//给定一个程序记得类Type
	cfg.AddMaps(new[] {
		typeof(Foo)
	});
});

var mapper = config.CreateMapper();
mapper.Map<FooDto>(foo).Dump();

//组织配置文件的方式。
public class OrganizationProfile : Profile
{
	public OrganizationProfile()
	{
		CreateMap<Foo, FooDto>();
		// Use CreateMap... Etc.. here (Profile methods are the same as configuration methods)

		//Naming Conventions （命名约定）
		//SourceMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
		//DestinationMemberNamingConvention = PascalCaseNamingConvention.Instance;
	}
}


class Foo
{
	public string Name { get; set; }

}

class FooDto : Foo
{
}