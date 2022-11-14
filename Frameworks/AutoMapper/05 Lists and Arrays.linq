<Query Kind="Program">
  <NuGetReference>AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//Lists and Arrays (列表和数组)

//AutoMapper 只需要配置元素类型，而不需要配置任何数组或列表类型。
void Main()
{
	var configuration = new MapperConfiguration(cfg => cfg.CreateMap<Source, Destination>());

	var sources = new[]
		{
		new Source { Value = 5 },
		new Source { Value = 6 },
		new Source { Value = 7 }
	};
	
	var mapper = configuration.CreateMapper();

	IEnumerable<Destination> ienumerableDest = mapper.Map<Source[], IEnumerable<Destination>>(sources);
	ICollection<Destination> icollectionDest = mapper.Map<Source[], ICollection<Destination>>(sources);
	IList<Destination> ilistDest = mapper.Map<Source[], IList<Destination>>(sources);
	List<Destination> listDest = mapper.Map<Source[], List<Destination>>(sources);
	Destination[] arrayDest = mapper.Map<Source[], Destination[]>(sources);
	
	ienumerableDest.Dump();
	icollectionDest.Dump();
	ilistDest.Dump();
	listDest.Dump();
	arrayDest.Dump();

	//处理空集合
	//映射集合属性时，如果源值为 null，则自动映射程序会将目标字段映射到空集合，而不是将目标值设置为 null。
	// 可以通过在配置映射器时将属性设置为 true 来更改此行为。

	configuration = new MapperConfiguration(cfg =>
	{
		cfg.AllowNullCollections = true;
		cfg.CreateMap<Source, Destination>();
	});
	
	mapper = configuration.CreateMapper();
	
	sources = null;
	arrayDest = mapper.Map<Source[], Destination[]>(sources);
	arrayDest.Dump();

	//Polymorphic element types in collections (集合中的多态元素类型)
	// 我们的源类型和目标类型中可能都有一个类的父子结构。AutoMapper 支持多态数组的映射。

	 configuration = new MapperConfiguration(c =>
	{
		c.CreateMap<ParentSource, ParentDestination>()
			 .Include<ChildSource, ChildDestination>()
			 ;
		c.CreateMap<ChildSource, ChildDestination>();
	});
	
	mapper = configuration.CreateMapper();

	var sources2 = new[]
	{
		new ParentSource(),
		new ChildSource(),
		new ParentSource()
	};
	
	// 转化时，ParentSource 是给可以用子类替换的。 加了Include配置之后。就可显示将子类进行一个转化。
	var destinations = mapper.Map<ParentSource[], ParentDestination[]>(sources2);
	destinations[0].Dump();	
	destinations[1].Dump();

}


public class Source
{
	public int Value { get; set; }
}

public class Destination
{
	public int Value { get; set; }
}

public class ParentSource
{
	public int Value1 { get; set; }
}

public class ChildSource : ParentSource
{
	public int Value2 { get; set; }
}

public class ParentDestination
{
	public int Value1 { get; set; }
}

public class ChildDestination : ParentDestination
{
	public int Value2 { get; set; }
}