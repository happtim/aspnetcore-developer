<Query Kind="Statements">
  <NuGetReference>FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/transform.html

//有些情况你需要 在验证之前做个转化 。
var validator = new PersonValidator();
var result = validator.Validate(new Person() {SomeStringProperty = "1"});
result.ToString().Dump();

public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		RuleFor(x => x.Name).NotNull();
		RuleFor(x => x.Email).NotNull();
		//比如有个 SomeStringProperty 属性，他代表一个int 类型的string。我们需要将他转化为int之后在进行验证。
		Transform(from: x => x.SomeStringProperty, to: value => int.TryParse(value, out int val) ? (int?)val : null)
			.GreaterThan(10);
	}
}

public class Person 
{
	public string Name { get; set; }
	public string Email { get; set; }
	public string SomeStringProperty {get;set;}
}