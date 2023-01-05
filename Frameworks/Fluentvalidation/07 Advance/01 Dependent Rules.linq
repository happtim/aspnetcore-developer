<Query Kind="Statements">
  <NuGetReference>FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/dependentrules.html


//FluentValidation 定义得规则默认都是相互独立执行验证得，这样可以方便异步验证。
//有些时候你希望一些规则验证 在某一个规则验证通过后才可以执行。可以使用 DependentRules 方法。

//作者说：就个人而言他不喜欢使用 DependentRules ，因为他写出来得代码可读性比较差。
//大部分得情况。可以使用 When 和 CascadeMode 联合起来 实现 DependentRules 相同得功能。虽然代码更重复了，但是可读性提高了。
var validator = new PersonValidator();
var result = validator.Validate(new Person());
result.ToString().Dump();

public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		//如下： Forename 得验证只有在 Surname 通过之后才能验证。
		RuleFor(person => person.Surname).NotNull().DependentRules(() =>
		{
			RuleFor(person => person.Forename).NotNull();
		});
	}
}

public class Person
{
	public string Surname { get; set; }
	public string Forename { get; set; }
}