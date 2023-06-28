<Query Kind="Statements">
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.Extensions.Primitives</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

 //https://learn.microsoft.com/en-us/dotnet/core/extensions/primitives
 
 //StringValues 有效方式表示 、零个、一个或多个字符串的类型。
 
var qs =  QueryString.Create("A","1");
qs.ToString().Dump();

qs = QueryString.Create(new Dictionary<string, StringValues>() { { "a", new StringValues(new string[] {"1","2"})}});
qs.ToString().Dump();


string 登飞来峰 = @"
飞来山上千寻塔，

闻说鸡鸣见日升。
不畏浮云遮望眼，
自缘身在最高层。";

StringValues values =new(登飞来峰.Split(new[] { '\n' }));
		
foreach (var line in values)
{
	line.Dump();
}
