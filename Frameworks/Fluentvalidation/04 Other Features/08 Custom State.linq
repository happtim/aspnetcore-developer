<Query Kind="Statements">
  <NuGetReference>FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
</Query>

// https://docs.fluentvalidation.net/en/latest/custom-state.html


//可以使用WithState 方法给指定的规则，设置一些自定义值。在验证的过程中可以返回一些上下文消息。
var validator = new PersonValidator();
var result = validator.Validate(new Person());
foreach (var failure in result.Errors)
{
	Console.WriteLine($"Property: {failure.PropertyName} State: {failure.CustomState}");
}

public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		RuleFor(person => person.Surname).NotNull();
		RuleFor(person => person.Forename).NotNull().WithState(person => 1234);
	}
}

public class Person
{
	public string Surname { get; set; }
	public string Forename { get; set; }
}