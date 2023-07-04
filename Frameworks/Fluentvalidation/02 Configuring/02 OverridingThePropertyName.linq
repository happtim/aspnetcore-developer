<Query Kind="Statements">
    <NuGetReference Version="11.5.2">FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/configuring.html#overriding-the-property-name
//属性重命名

Customer customer = new Customer() { Forename = "foo" };
CustomerValidator validator = new CustomerValidator();

ValidationResult result = validator.Validate(customer);

result.Dump();


public class CustomerValidator : AbstractValidator<Customer>
{
	public CustomerValidator()
	{
		//使用WithName方法改变异常消息中的属性名称。此方法只是将ErrorMessage信息变更。异常属性名称未变更
		RuleFor(customer => customer.Surname).NotNull().WithName("Last name");
		//如果想变更属性名称 需要使用如下方法。
		//.OverridePropertyName("Last name");
		
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