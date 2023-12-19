<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//Lists and Arrays (列表和数组)

//Polymorphic element types in collections (集合中的多态元素类型)
// 我们的源类型和目标类型中可能都有一个类的父子结构。AutoMapper 支持多态数组的映射。

var configuration = new MapperConfiguration(c =>
{
   c.CreateMap<ParentSource, ParentDestination>()
		.Include<ChildSource, ChildDestination>()
		;
   c.CreateMap<ChildSource, ChildDestination>();
});

var mapper = configuration.CreateMapper();

var sources = new[]
{
	new ParentSource(),
	new ChildSource(),
	new ParentSource()
};

// 转化时，ParentSource 是给可以用子类替换的。 加了Include配置之后。就可显示将子类进行一个转化。
var destinations = mapper.Map<ParentSource[], ParentDestination[]>(sources);

destinations[0].Dump();
destinations[1].Dump();
destinations[2].Dump();


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