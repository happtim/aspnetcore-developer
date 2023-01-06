<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

//https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm

//使用与 .NET 对象不完全匹配的 JSON 时，在 .NET 对象之间手动序列化和反序列化非常有用。
string jsonText = @"{
  'short': {
    'original': 'http://www.foo.com/',
    'short': 'krehqk',
    'error': {
      'code': 0,
      'msg': 'No action taken'
    }
  }
}";

JObject json = JObject.Parse(jsonText);

Shortie shortie = new Shortie
{
Original = (string)json["short"]["original"],
Short = (string)json["short"]["short"],
Error = new ShortieException
{
Code = (int)json["short"]["error"]["code"],
ErrorMessage = (string)json["short"]["error"]["msg"]
}
};

Console.WriteLine(shortie.Original);
// http://www.foo.com/

Console.WriteLine(shortie.Error.ErrorMessage);

public class Shortie
{
	public string Original { get; set; }
	public string Shortened { get; set; }
	public string Short { get; set; }
	public ShortieException Error { get; set; }
}

public class ShortieException
{
	public int Code { get; set; }
	public string ErrorMessage { get; set; }
}