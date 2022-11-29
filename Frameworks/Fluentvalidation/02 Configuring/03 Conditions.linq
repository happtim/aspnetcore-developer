<Query Kind="Statements">
  <NuGetReference>FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/conditions.html

//When 和 Unless 方法可以在执行RuleFor进行条件控制

Customer customer = new Customer() {IsPreferredCustomer = true };
CustomerValidator validator = new CustomerValidator();

ValidationResult result = validator.Validate(customer);

result.ToString().Dump();


public class CustomerValidator : AbstractValidator<Customer>
{
	public CustomerValidator()
	{
		//When 当条件为true执行
		RuleFor(customer => customer.CustomerDiscount).GreaterThan(0).When(customer => customer.IsPreferredCustomer);
		//Unless 为 When 的反面 。当条件为false执行
		RuleFor(customer => customer.CustomerDiscount).GreaterThan(0).Unless(customer => !customer.IsPreferredCustomer);

		//如果需要相同的一个条件在多个RuleFor。可以通过When方法避免写很多个规则。
		When(customer => customer.IsPreferredCustomer, () =>
		{
			RuleFor(customer => customer.CustomerDiscount).GreaterThan(0);
			RuleFor(customer => customer.CreditCardNumber).NotNull();
			//可以使用Otherwise方法在不满足条件的时候验证
		}).Otherwise(() => {
			 RuleFor(customer => customer.CustomerDiscount).Equal(0);
		});
		
		//默认情况When命令将之前所有的Validator都会过滤。如果你想只限制在当前的这个验证器，需要增加一个CurrentValidator参数。
		RuleFor(customer => customer.CustomerDiscount)
			.GreaterThan(0).When(customer => customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator)
			.Equal(0).When(customer => !customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator);
		
		//When 只提供一个条件判断。如下When1只提供Matches的条件判断。并不判断NotEmpty。
		RuleFor(customer => customer.Photo)
			.NotEmpty()
			// .When(customer => customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator)
			.Matches("https://wwww.photos.io/\\d+\\.png")
			.When(customer => customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator)
			.Empty()
			.When(customer => !customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator);
	}
}


public class Customer
{
	public int Id { get; set; }
	public string Surname { get; set; }
	public string Forename { get; set; }
	public decimal CustomerDiscount { get; set; }
	public string Address { get; set; }
	public bool IsPreferredCustomer { get; set; }
	public string CreditCardNumber { get; set; }
	public string Photo {get;set;}
}