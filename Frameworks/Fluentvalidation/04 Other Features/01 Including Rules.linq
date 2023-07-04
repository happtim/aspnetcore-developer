<Query Kind="Statements">
  <NuGetReference Version="11.5.2">FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/including-rules.html

//一个验证其可以引用同类型得其他验证器，这样得好处就可以将一个类得验证分成多个文件中。

Person person = new Person();
person.Surname = "Ge";
person.Forename = "Tim";
PersonValidator validator = new PersonValidator();
ValidationResult result = validator.Validate(person);

result.ToString().Dump();

public class PersonAgeValidator : AbstractValidator<Person>
{
	public PersonAgeValidator()
	{
		RuleFor(x => x.DateOfBirth).Must(BeOver18);
	}

	protected bool BeOver18(DateTime date)
	{
		return DateTime.Now.Subtract(date).TotalDays > 18*365;
	}
}

public class PersonNameValidator : AbstractValidator<Person>
{
	public PersonNameValidator()
	{
		RuleFor(x => x.Surname).NotNull().Length(0, 255);
		RuleFor(x => x.Forename).NotNull().Length(0, 255);
	}
}

//以上两个验证其都验证得一个对象， 所以可以将他们合并成一个。

public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		Include(new PersonAgeValidator());
		Include(new PersonNameValidator());
	}
}


public class Person
{
	public string Surname {get;set;}
	public string Forename {get;set;}
	public DateTime DateOfBirth {get;set;} 
}