<Query Kind="Statements">
  <NuGetReference Version="11.5.2">FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/advanced.html

//这些功能通常不用于日常使用，但提供了一些在某些情况下可能有用的其他扩展点。

// 对于高级用户，可以将任意数据传递到可从自定义属性验证程序中。
// 这样可以是的验证其 基于任意的传入数据 做出验证判断。
var person = new Person();
var context = new ValidationContext<Person>(person);
context.RootContextData["MyCustomData"] = "Test";
var validator = new PersonValidator();
var result = validator.Validate(context);
result.ToString().Dump();

public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		RuleFor(x => x.Surname).Custom((x, context) =>
		{
			if (context.RootContextData.ContainsKey("MyCustomData"))
			{
				context.AddFailure("My error message");
			}
		});
	}
}


public class Person
{
	public string Surname { get; set; }
	public string Forename { get; set; }
}