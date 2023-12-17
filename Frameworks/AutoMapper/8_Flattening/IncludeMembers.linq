<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//Flattening (平坦化)

//IncludeMembers 
// 你可以直接将子类型和目标类型进行映射。 然后就可以通过IncludeMembers方法 子对象的成员映射到目标对象。
// 这样的一个好处就是可以用重用子类型的映射到目标类型。

var configuration = new MapperConfiguration(cfg =>
{
	//IncludeMembers函数的参数顺序是对映射结果有相关的。优先匹配参数结果的第一个出现的映射。
	cfg.CreateMap<Source, Destination>().IncludeMembers(s => s.InnerSource, s => s.OtherInnerSource);
	//如果将次序更换结果将不同
	//cfg.CreateMap<Source, Destination>().IncludeMembers(s => s.OtherInnerSource, s => s.InnerSource);
	
	cfg.CreateMap<InnerSource, Destination>(MemberList.None);
	cfg.CreateMap<OtherInnerSource, Destination>();
});

var mapper = configuration.CreateMapper();

var source = new Source
{
	Name = "name_Source",
	InnerSource = new InnerSource { Description = "description_InnerSource", Name = "name_InnerSource" },
	OtherInnerSource = new OtherInnerSource { Title = "title_OtherInnerSource", Name = "name_OtherInnerSource" ,Description = "description_OtherInnerSource"}
};

var destination = mapper.Map<Destination>(source);

destination.Dump();


public class Source
{
	public string Name { get; set; }
	public InnerSource InnerSource { get; set; }
	public OtherInnerSource OtherInnerSource { get; set; }
}

public class InnerSource
{
	public string Name { get; set; }
	public string Description { get; set; }
}

public class OtherInnerSource
{
	public string Name { get; set; }
	public string Description { get; set; }
	public string Title { get; set; }
}

public class Destination
{
	public string Name { get; set; }
	public string Description { get; set; }
	public string Title { get; set; }
}
