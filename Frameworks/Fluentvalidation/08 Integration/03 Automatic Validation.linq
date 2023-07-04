<Query Kind="Statements">
  <NuGetReference Version="11.5.2">FluentValidation</NuGetReference>
  <NuGetReference Version="11.3.0">FluentValidation.AspNetCore</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.Swagger</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerGen</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerUI</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.AspNetCore</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.AspNetCore.Mvc</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

//https://docs.fluentvalidation.net/en/latest/aspnet.html#automatic-validation

//swagger地址
//http://localhost:5000/swagger/index.html

//AspNetCore自动处理验证 需要安装 FluentValidation.AspNetCore 包
//Install-Package FluentValidation.AspNetCore 

//通过自动验证，FluentValidation 插入到作为核心 MVC 一部分 ASP.NET 验证管道，并允许在调用控制器操作之前（在模型绑定期间）验证模型。
//这种验证方法更加无缝，但有几个缺点：

//自动验证不是异步的：如果验证程序包含异步规则，则验证程序将无法运行。如果尝试使用具有自动验证功能的异步验证程序，则会在运行时收到异常。
//自动验证仅适用于 MVC：自动验证仅适用于 MVC 控制器和 Razor 页面。它不适用于 ASP.NET 的更现代的部分，例如最小 API 或 Blazor。
//自动验证很难调试：自动验证的“神奇”性质使得如果出现问题，很难进行调试/故障排除，因为幕后做了很多事情。

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers()
	.AddApplicationPart(typeof(PersonController).Assembly);
	
builder.Services.AddScoped<IValidator<Person>, PersonValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();


[Route("api/[controller]")]
[ApiController]
public class PersonController : ControllerBase
{

	private IValidator<Person> _validator;
	public PersonController(IValidator<Person> validator)
	{
		_validator = validator;
	}

	[HttpPost("Create")]
	public async Task<IActionResult> Create(Person person)
	{

		if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ModelState);
        }

        return StatusCode(StatusCodes.Status200OK, "Model is valid!");
	}
}


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