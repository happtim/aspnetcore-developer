<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper</NuGetReference>
  <Namespace>AutoMapper</Namespace>
</Query>


Source sou = new Source {frmValue = 10, frmValue2 = 20};

//Recognizing pre/postfixes （识别前/后缀）
// 
var config = new MapperConfiguration(cfg =>
{
		// 直接设置配置。
	cfg.CreateMap<Source, Dest>();
	
	cfg.RecognizePrefixes("frm");  //识别源的前缀 省略了Source
	//cfg.RecognizeDestinationPrefixes("Temp"); //识别目标的前缀
});
var mapper = config.CreateMapper();
mapper.Map<Dest>(sou).Dump();


public class Source
{
	public int frmValue { get; set; }
	public int frmValue2 { get; set; }
}
public class Dest
{
	public int Value { get; set; }
	public int Value2 { get; set; }
}