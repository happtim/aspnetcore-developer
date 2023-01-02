<Query Kind="Statements">
  <NuGetReference>FluentValidation</NuGetReference>
  <NuGetReference>FluentValidation.DependencyInjectionExtensions</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

var builder = WebApplication.CreateBuilder();

builder.Services.AddHostedService<UserHostedService>();

//注册IValidator<T> 类型 和 类型验证器
builder.Services.AddScoped<IValidator<User>,UserValidator>();

//也可以通过 FluentValidation.DependencyInjectionExtensions 包中的扩展方法进行自动注册
//通过泛型的方式自动加载注册
//builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

//通过类型的方式自动加载注册
//builder.Services.AddValidatorsFromAssemblyContaining(typeof(UserValidator));

//通过Assembly的方式自动加载注册
//builder.Services.AddValidatorsFromAssembly(Assembly.Load("LINQPadQuery"));

var app = builder.Build();

app.Run();


public class UserHostedService : IHostedService
{
	private readonly IValidator<User> _validator;

	public UserHostedService(IValidator<User> validator)
	{
		_validator = validator;
	}

	public async Task DoSomething(User user)
	{
		var result = await _validator.ValidateAsync(user);
		
		result.ToString().Dump();
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await DoSomething(new User());
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}

public class UserValidator : AbstractValidator<User>
{
	public UserValidator()
	{
		RuleFor(x => x.Name).NotNull();
	}
}

public class User
{
	public string Name {get;set;}
}