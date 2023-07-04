<Query Kind="Statements">
    <NuGetReference Version="11.5.2">FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/configuring.html


Customer customer = new Customer() {Forename = "foo"};
CustomerValidator validator = new CustomerValidator();

ValidationResult result = validator.Validate(customer);

result.ToString().Dump();


public class CustomerValidator : AbstractValidator<Customer>
{
	public CustomerValidator()
	{
		//使用WithMessage方法重写错误提示信息，可以使用{PropertyName}占位符代表属性名称
		RuleFor(customer => customer.Surname).NotNull()
			.WithMessage("Please ensure that you have entered your {PropertyName} but value is {PropertyValue}");
			//还可以使用自定义参数的Format的方式输出提示信息。
			//.WithMessage(customer =>  string.Format("This message references some constant values: {0} {1}", "hello", 5));
		//{PropertyValue} 占位符也可适用于各种验证器。
		RuleFor(customer => customer.Forename).NotEqual("foo").WithMessage("Please ensure that you have entered your {PropertyName} value is not {PropertyValue}");
		//Equal NotEqual GreaterThan GreaterThanOrEqual 这些验证器中可以使用{ComparisonValue} {ComparisonProperty} 这两个占位符。
		RuleFor(customer => customer.Surname).Equal(customer => customer.Forename).WithMessage("Please ensure that you have entered your {ComparisonProperty}'s value is equal {PropertyName}'s value");
		//Length 验证器可以使用 {MinLength} {MaxLength} {TotalLength} 这些占位符。
		RuleFor(customer => customer.Forename).Length(5,6).WithMessage("Please ensure that you have entered your {PropertyName}'s value between {MinLength} and {MaxLength}");
	}
}

public class Customer
{
	public int Id { get; set; }
	public string Surname { get; set; }
	public string Forename { get; set; }
	public decimal Discount { get; set; }
	public string Address { get; set; }
}