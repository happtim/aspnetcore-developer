<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>

//Lists and Arrays (列表和数组)

//处理空集合
//映射集合属性时，如果源值为 null，则自动映射程序会将目标字段映射到空集合，而不是将目标值设置为 null。
// 可以通过在配置映射器时将属性设置为 true 来更改此行为。

var configuration = new MapperConfiguration(cfg =>
{
	cfg.AllowNullCollections = true;
	cfg.CreateMap<Source, Destination>();
});

var mapper = configuration.CreateMapper();

Source[] sources = null;
var arrayDest = mapper.Map<Source[], Destination[]>(sources);
arrayDest.Dump();



public class Source
{
	public int Value { get; set; }
}

public class Destination
{
	public int Value { get; set; }
}
