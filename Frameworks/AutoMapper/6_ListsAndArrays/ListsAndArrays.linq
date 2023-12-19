<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//Lists and Arrays (列表和数组)

//AutoMapper 只需要配置元素类型，而不需要配置任何数组或列表类型。

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



public class Source
{
	public int Value { get; set; }
}

public class Destination
{
	public int Value { get; set; }
}
