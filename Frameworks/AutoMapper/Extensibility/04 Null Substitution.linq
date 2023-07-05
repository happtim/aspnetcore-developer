<Query Kind="Program">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//Null Substitution （空值处理）

// 如果在源中有空值，那么可以使用配置将目标属性设置指定值。

void Main()
{
	var config = new MapperConfiguration(cfg =>
		cfg.CreateMap<Source, Destination>()
			.ForMember(destination => destination.Value, opt => opt.NullSubstitute("Other Value")));
			
	var source = new Source { Value = null };
	var mapper = config.CreateMapper();
	
	var dest = mapper.Map<Source, Destination>(source);
	dest.Dump();
	
	source.Value = "Not null";
	dest = mapper.Map<Source, Destination>(source);
	dest.Dump();

}

public class Source
{
	public string Value { get; set; }
}

public class Destination
{
	public string Value { get; set; }
}