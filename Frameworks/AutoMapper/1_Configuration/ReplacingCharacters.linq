<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>


var sou = new Source() { Value = 10, Ävíator = 20, SubAirlinaFlight = 30};

//Replacing characters （替换字符）
// 可以在成员名称匹配期间替换源成员中的单个字符或整个单词
var config = new MapperConfiguration(cfg =>
{
		// 直接设置配置。
	cfg.CreateMap<Source, Destination>();
	cfg.ReplaceMemberName("Ä", "A");
	cfg.ReplaceMemberName("í", "i");
	cfg.ReplaceMemberName("Airlina", "Airline");
});

var mapper = config.CreateMapper();
mapper.Map<Destination>(sou).Dump();


public class Source
{
	public int Value { get; set; }
	public int Ävíator { get; set; }
	public int SubAirlinaFlight { get; set; }
}
public class Destination
{
	public int Value { get; set; }
	public int Aviator { get; set; }
	public int SubAirlineFlight { get; set; }
}