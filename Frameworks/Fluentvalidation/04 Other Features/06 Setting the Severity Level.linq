<Query Kind="Statements">
  <NuGetReference Version="11.5.2">FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/severity.html

//默认情况下验证器失败了使用ERROR严重级别。
var person = new Person();
var validator = new PersonValidator();
var result =  validator.Validate(person);

//也可以通过配置全局参数的方式配置严重级别
//ValidatorOptions.Global.Severity = Severity.Info;

foreach (var failure in result.Errors)
{
	Console.WriteLine($"Property: {failure.PropertyName} Severity: {failure.Severity}");
}

public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		//可以对验证的字段设置严重级别为Warning ，ValidationResult 中的 Errors错误列表中级别就是Warning。
		RuleFor(person => person.Surname).NotNull().WithSeverity(person => Severity.Warning);
		RuleFor(person => person.Forename).NotNull();
	}
}


public class Person
{
	public string Surname {get;set;}
	public string Forename {get;set;}
} 