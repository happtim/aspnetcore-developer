<Query Kind="Statements" />


//通过调用 Regex.IsMatch 方法确定输入文本中是否具有正则表达式模式。
var email = "526821398@qq.com";

Regex.IsMatch(email,
				   @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
				   RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)).Dump();
				   
//通过调用 Regex.Match 或 Regex.Matches 方法检索匹配正则表达式模式的一个或所有文本匹配项。

string str="AGV/cefd54f4-3c41-19d0-08a9-4dc69a674013/identity";

var match = Regex.Match(str, "AGV/(.*)/identity");

match.Dump();

//通过调用 Regex.Replace 方法替换匹配正则表达式模式的文本。
var strIn = "Hello World";
 Regex.Replace(strIn, @"[^\w\.@-]", "",
							   RegexOptions.None, TimeSpan.FromSeconds(1.5)).Dump();