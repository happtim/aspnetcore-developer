<Query Kind="Statements">
  <NuGetReference Version="11.5.2">FluentValidation</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.Swagger</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerGen</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerUI</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.OpenApi.Models</Namespace>
  <Namespace>Microsoft.AspNetCore.Mvc</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

//https://docs.fluentvalidation.net/en/latest/aspnet.html#manual-validation

//swagger地址
//http://localhost:5000/swagger/index.html


//通过手动验证，您可以将验证程序注入控制器（或 api 端点），
//调用验证程序并根据结果进行操作。这是最直接和可靠的方法。

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers()
	.AddApplicationPart(typeof(PersonController).Assembly);
builder.Services.AddScoped<IValidator<Person>, PersonValidator>();

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
		
		ValidationResult result = await _validator.ValidateAsync(person);
		
		if (!result.IsValid)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
			}
			
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
