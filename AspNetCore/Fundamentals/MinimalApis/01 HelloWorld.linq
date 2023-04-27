<Query Kind="Statements">
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

//构建最小 API，以创建具有最小依赖项的 HTTP API。 
//它们非常适合于需要在 ASP.NET Core 中仅包括最少文件、功能和依赖项的微服务和应用。

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();