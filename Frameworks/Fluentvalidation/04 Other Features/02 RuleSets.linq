<Query Kind="Statements">
  <NuGetReference Version="11.5.2">FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
</Query>

// https://docs.fluentvalidation.net/en/latest/rulesets.html

// RuleSet 可以给验证规则分组，这些规则可以作为一个组一起执行，同时忽略其他规则。
// 这样做这允许您将复杂的验证程序定义分解为可以单独执行的较小段。

Person person = new Person();
person.Surname = "Ge"; 
//person.Forename = "Tim";
PersonValidator validator = new PersonValidator();

//如下只验证规则集Names下得Surname Forename。 
ValidationResult result =
	validator.Validate(person, options => options.IncludeRuleSets("Names"));
	
	//可以指定多个规则集
	//validator.Validate(person, options => options.IncludeRuleSets("Names", "MyRuleSet", "SomeOtherRuleSet"));
	
	//如果你还使用原来得 validator.Validate(person); 则只验证不属于规则得规则。
	//validator.Validate(person);
	
	//在使用Names得规则集下，又添加“default”得规则集
	//validator.Validate(person, options => options.IncludeRuleSets("Names").IncludeRulesNotInRuleSet());
	
	//使用全部得验证规则 包括
	//validator.Validate(person,options => options.IncludeAllRuleSets());

result.ToString().Dump();

//按照名字验证
public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		RuleSet("Names", () =>
		{
			RuleFor(x => x.Surname).NotNull();
			RuleFor(x => x.Forename).NotNull();
		});

		RuleFor(x => x.Id).NotEqual(0);
	}
}

public class Person
{
	public int Id {get;set;}
	public string Surname { get; set; }  //姓
	public string Forename { get; set; } //名
	public DateTime DateOfBirth { get; set; }
}