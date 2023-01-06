<Query Kind="Statements">
  <NuGetReference>FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

//https://docs.fluentvalidation.net/en/latest/aspnet.html

//FluentValidation 可以在 ASP.NET Core Web 应用程序中用于验证传入的模型。有两种主要方法可以执行此操作：
//手动验证
//自动验证

var builder = WebApplication.CreateBuilder();

//注册IValidator<T> 类型 和 类型验证器
//也可以通过  04 Other Features/ 04 Dependency Injection 依赖注入的其他方式
builder.Services.AddScoped<IValidator<Person>, PersonValidator>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();


public class Person
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public int Age { get; set; }
}

public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		RuleFor(x => x.Id).NotNull();
		RuleFor(x => x.Name).Length(0, 10);
		RuleFor(x => x.Email).EmailAddress();
		RuleFor(x => x.Age).InclusiveBetween(18, 60);
	}
}
