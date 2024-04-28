<Query Kind="Statements">
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

//WebApplication.CreateBuilder(): 目前.Net 6中最新的使用方式。
//之前的版本都将应用的配置分配到两个文件中: Program.cs和Startup.cs, 
//但是在.Net 6中这一切都被改变了，我们之前的文章都学习过了，所有的事情都在一个文件中全部搞定：

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();