<Query Kind="Statements">
  <NuGetReference Version="11.5.2">FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/advanced.html

//这些功能通常不用于日常使用，但提供了一些在某些情况下可能有用的其他扩展点。

//如果需要在每次调用验证程序时运行特定代码，可以通过重写 PreValidate 方法来执行此操作。
//此方法使用 ValidationContext 和 ValidationResult，您可以使用它们来自定义验证过程。
//如果验证应继续，则该方法应返回 true，如果立即中止，则该方法应返回 false。您对验证结果所做的任何修改都将返回给用户。
var validator = new PersonValidator();
var result = validator.Validate((Person)null);
result.ToString().Dump();


public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		RuleFor(x => x.Surname).NotNull();
	}
	
	//请注意，此方法是在 FluentValidation 对正在验证的模型执行其标准 null 检查之前调用的
	//因此如果模型为 null，您可以使用此方法生成错误
	protected override bool PreValidate(ValidationContext<Person> context, ValidationResult result)
	{
		if (context.InstanceToValidate == null)
		{
			result.Errors.Add(new ValidationFailure("", "Please ensure a model was supplied."));
			return false;
		}
		return true;
	}
}


public class Person
{
	public string Surname { get; set; }
	public string Forename { get; set; }
}