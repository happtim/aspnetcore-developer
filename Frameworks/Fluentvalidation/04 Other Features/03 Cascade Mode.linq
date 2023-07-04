<Query Kind="Statements">
  <<NuGetReference Version="11.5.2">FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/cascade.html


Person person = new Person();
//person.Surname = "Ge";
//person.Forename = "Tim";
PersonValidator validator = new PersonValidator();

//设置 全局级别得
//ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
//ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

ValidationResult result = validator.Validate(person);
	
result.ToString().Dump();

public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		//假设我们有个两个验证器，一个NotNull，一个NotEqual。默认当Surname为null时，也会执行第二个验证器。
		RuleFor(x => x.Surname).NotNull().Equal("foo");
		RuleFor(x => x.Forename).NotNull().Equal("foo");
		
		//如果我们加上了CascadeMode.Stop 模式之后将有个一个验证器失败了，就不会往后面执行了。
		//CascadeMode有两个值：Continue和Stop。默认试用Continue会执行所有得验证器。
		//RuleFor(x => x.Surname).Cascade(CascadeMode.Stop).NotNull().Equal("foo");
		
		//rule-level为了避免多个变量得验证都设置CascadeMode.Stop。有一个全局得变量可以设置。
		RuleLevelCascadeMode = CascadeMode.Stop;
		
		//class-level 如果有个多个变量验证，只要有一个验证失败，就返回失败，其余后续几个不会
		ClassLevelCascadeMode = CascadeMode.Stop;
		
		
	}
}


public class Person
{
	public int Id { get; set; }
	public string Surname { get; set; }  //姓
	public string Forename { get; set; } //名
	public DateTime DateOfBirth { get; set; }
}