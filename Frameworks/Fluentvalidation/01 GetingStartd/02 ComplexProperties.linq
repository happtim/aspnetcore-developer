<Query Kind="Statements">
  <NuGetReference>FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
</Query>


Customer customer = new Customer() {Address = new Address()};
CustomerValidator validator = new CustomerValidator();
//使用Validate方法来运行validator
ValidationResult result = validator.Validate(customer);
result.ToString().Dump();

//使用复杂类型的验证。
public class CustomerValidator : AbstractValidator<Customer>
{
	public CustomerValidator()
	{
		RuleFor(customer => customer.Name).NotNull();
		RuleFor(customer => customer.Address).SetValidator(new AddressValidator());
		//当子属性为空，Validator就不会执行了。 使用构造函数 Customer customer = new Customer();
		
		//抛出空指针异常。
		//RuleFor(customer => customer.Address.Postcode).NotNull();
		//如果不抛出控制异常。
		//RuleFor(customer => customer.Address.Postcode).NotNull().When(customer => customer.Address != null);
	}
}

public class AddressValidator : AbstractValidator<Address>
{
	public AddressValidator()
	{
		RuleFor(address => address.Postcode).NotNull();
		//etc
	}
}

public class Customer
{
	public string Name { get; set; }
	public Address Address { get; set; }
}

public class Address
{
	public string Line1 { get; set; }
	public string Line2 { get; set; }
	public string Town { get; set; }
	public string County { get; set; }
	public string Postcode { get; set; }
}