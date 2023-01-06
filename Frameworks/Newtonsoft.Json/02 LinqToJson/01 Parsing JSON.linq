<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

//https://www.newtonsoft.com/json/help/html/ParsingLINQtoJSON.htm

//LINQ to JSON 具有可用于从字符串解析 JSON 或直接从文件加载 JSON 的方法。

//解析 JSON Object 从文本
string json = @"{
  CPU: 'Intel',
  Drives: [
    'DVD read/writer',
    '500 gigabyte hard drive'
  ]
}";

JObject o = JObject.Parse(json);

o.Dump();

//解析 JSON Array 从文本
json = @"[
  'Small',
  'Medium',
  'Large'
]";

JArray a = JArray.Parse(json);

a.Dump();

//解析 JSON Object 从文件
using (StreamReader reader = File.OpenText(@"c:\person.json"))
{
	JObject oo = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
	// do stuff
}