<Query Kind="Statements">
  <NuGetReference>FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>Microsoft.Extensions.Localization</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

//https://docs.fluentvalidation.net/en/latest/localization.html

//Fluentvalidation 开箱即用，提供了默认很多语言的错误消息。默认的语言使用的是 CultureInfo.CurrentUICulture。

var cultureInfo = CultureInfo.CurrentUICulture;
cultureInfo.Dump();

var validator = new PersonValidator();
var result = validator.Validate(new Person());
result.ToString().Dump();

public class PersonValidator : AbstractValidator<Person>
{
	//private readonly IStringLocalizer<Person> _localizer;
	public PersonValidator()
	{
		//通过WithMessage方法重写消息
		RuleFor(person => person.Surname).NotNull().WithMessage(x => "Surname Required");
		//ASP.Net Core 可以是使用IStringLocalizer 依赖注入方式本地化错误信息。
		//RuleFor(person => person.Forename).NotNull().WithMessage(x => _localizer["Forename is required"]);
		RuleFor(person => person.Forename).NotNull();
	}
}

public class Person
{
	public string Surname { get; set; }
	public string Forename { get; set; }
}