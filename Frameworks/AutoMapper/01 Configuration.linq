<Query Kind="Program">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

void Main()
{
	//MapperConfiguration 创建一个实例并通过构造函数初始化配置
	//MapperConfiguration 实例可以静态存储在静态字段或依赖项注入容器中。一旦创建，就无法更改/修改。
	var foo = new Foo(){Name ="Foo"};
		
	var config = new MapperConfiguration(cfg =>
	{
		// 直接设置配置。
		cfg.CreateMap<Foo, FooDto>();
	});
	
	var mapper =  config.CreateMapper();
	mapper.Map<FooDto>(foo).Dump();

	//Profile Instances 配置实体
	//组织映射配置的一个好方法是使用配置文件。 创建继承自并将配置放在构造函数中的类：Profile

	config = new MapperConfiguration(cfg =>
	{
		// 使用配置文件。
		cfg.AddProfile<OrganizationProfile>();
		// 另一种使用配置文件。
		cfg.AddProfile(new OrganizationProfile());
	});

	mapper = config.CreateMapper();
	mapper.Map<FooDto>(foo).Dump();

	//Assembly Scanning for auto configuration （用于自动配置的装配扫描）
	//AutoMapper 将扫描指定的程序集以查找从配置文件继承的类，并将其添加到配置中。
	var myAssembly = Assembly.GetExecutingAssembly();
	config = new MapperConfiguration(cfg =>
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
	

	mapper = config.CreateMapper();
	mapper.Map<FooDto>(foo).Dump();

	//Naming Conventions （命名约定）
	//下面配置这会将以下属性相互映射： property_name -> PropertyName
	//如果不需要命名约定，可以使用。ExactMatchNamingConvention
	//命名约定就三个类LowerUnderscoreNamingConvention，PascalCaseNamingConvention，ExactMatchNamingConvention
	config = new MapperConfiguration(cfg =>
	{
		// 直接设置配置。
		cfg.CreateMap<Foo, FooDto>();
		cfg.SourceMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
		cfg.DestinationMemberNamingConvention = PascalCaseNamingConvention.Instance;
	});
	
	foo.property_name =  "命名约定";
	mapper = config.CreateMapper();
	mapper.Map<FooDto>(foo).Dump();


	//Replacing characters （替换字符）
	// 可以在成员名称匹配期间替换源成员中的单个字符或整个单词
	foo.Ävíator = 10;
	config = new MapperConfiguration(cfg =>
	{
		// 直接设置配置。
		cfg.CreateMap<Foo, FooDto>();
		cfg.ReplaceMemberName("Ä", "A");
		cfg.ReplaceMemberName("í", "i");
	});
	mapper = config.CreateMapper();
	mapper.Map<FooDto>(foo).Dump();

	//Recognizing pre/postfixes （识别前/后缀）
	// 
	config = new MapperConfiguration(cfg =>
	{
		// 直接设置配置。
		cfg.CreateMap<Foo, FooDto>();
		//cfg.RecognizePrefixes("Temp"); 识别源的前缀
		cfg.RecognizeDestinationPrefixes("Temp"); //识别目标的前缀
	});
	mapper = config.CreateMapper();
	mapper.Map<FooDto>(foo).Dump();

	//Global property/field filtering （全局属性/字段筛选）
	// 默认情况下，自动映射器会尝试映射每个公共属性/字段。您可以使用属性/字段筛选器筛选出属性/字段：
	config = new MapperConfiguration(cfg =>
	{
		// 直接设置配置。
		cfg.CreateMap<Foo, FooDto>();
		// 禁止所有Field 字段映射
		cfg.ShouldMapField = fi => false;

		// 映射有公用get方法或者私有get方法的属性。
		cfg.ShouldMapProperty = pi =>
			pi.GetMethod != null && (pi.GetMethod.IsPublic || pi.GetMethod.IsPrivate);
	});
	foo.FieldName = "ShouldMapField=>false";
	foo.PrivateProperty = "ShouldMapProperty => pi.GetMethod.IsPrivate";
	mapper = config.CreateMapper();
	mapper.Map<FooDto>(foo).Dump();

}

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
	public string property_name { get; set; }
	public int Ävíator { get; set; }
	public string FieldName;
	public string PrivateProperty {private get;set;}
}

class FooDto :Foo
{
	public int Aviator { get; set; }
	public string PropertyName {get;set;}
	public string TempName {get;set;}
	public new string PrivateProperty {get;set;}
}