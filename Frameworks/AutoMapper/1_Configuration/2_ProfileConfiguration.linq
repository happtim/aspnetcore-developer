<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//MapperConfiguration 创建一个实例并通过构造函数初始化配置
//MapperConfiguration 实例可以静态存储在静态字段或依赖项注入容器中。一旦创建，就无法更改/修改。
var foo = new Foo() { Name = "Foo" };

//Profile Instances 配置实体
//组织映射配置的一个好方法是使用配置文件。 创建继承自并将配置放在构造函数中的类：Profile

var config = new MapperConfiguration(cfg =>
{
		// 使用配置文件。
	cfg.AddProfile<OrganizationProfile>();
		// 另一种使用配置文件。
	cfg.AddProfile(new OrganizationProfile());
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