<Query Kind="Statements">
    <NuGetReference Version="11.5.2">FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/built-in-validators.html#

//FluentValidation 有很多内置的验证器。

Customer customer = new Customer() { IsPreferredCustomer = true };
CustomerValidator validator = new CustomerValidator();

ValidationResult result = validator.Validate(customer);

result.ToString().Dump();

public class CustomerValidator : AbstractValidator<Customer>
{
	public CustomerValidator()
	{
		//确保属性不为null
		RuleFor(customer => customer.Surname).NotNull();
		//确保值为null
		RuleFor(x => x.Surname).Null();
		//确保值不为null && 不为默认值。int：0 ， string：""; int[] 空数组。
		RuleFor(customer => customer.Surname).NotEmpty();
		//确保值为null || 为默认值。
		RuleFor(x => x.Surname).Empty();
		//确保属性值不等于给定值。
		RuleFor(customer => customer.Surname).NotEqual("Foo");
		//Not equal to another property
		RuleFor(customer => customer.Surname).NotEqual(customer => customer.Forename);
		//确保值等于给定值。
		RuleFor(customer => customer.Surname).Equal("Foo");
		//确保string值在指定范围内。
		RuleFor(customer => customer.Surname).Length(1, 250);
		//确保string的值小于指定长度
		RuleFor(customer => customer.Surname).MaximumLength(250);
		//确保string的值大于指定长度
		RuleFor(customer => customer.Surname).MinimumLength(10);
		//确保给定值小于某值
		RuleFor(customer => customer.CreditLimit).LessThan(100);
		//Less than another property
		RuleFor(customer => customer.CreditLimit).LessThan(customer => customer.MaxCreditLimit);
		//确保值小于等于某值
		RuleFor(customer => customer.CreditLimit).LessThanOrEqualTo(100);
		//Less than another property
		RuleFor(customer => customer.CreditLimit).LessThanOrEqualTo(customer => customer.MaxCreditLimit);
		//确保值大于于某值
		RuleFor(customer => customer.CreditLimit).GreaterThan(0);
		//确保值大于等于某值
		RuleFor(customer => customer.CreditLimit).GreaterThanOrEqualTo(1);
		//可以使用lambda表达式验证为true
		RuleFor(customer => customer.Surname).Must(surname => surname == "Foo");
		//确保值匹配正则表达式
		RuleFor(customer => customer.Surname).Matches("some regex here");
		//确保值为email
		RuleFor(customer => customer.Email).EmailAddress();
		//确保值为信用卡值
		RuleFor(x => x.CreditCardNumber).CreditCard();
		//确保值枚举类型值真实性。
		RuleFor(x => x.ErrorLevel).IsInEnum();
		//确保string值式一个枚举类型名称
		RuleFor(x => x.ErrorLevelName).IsEnumName(typeof(ErrorLevel));
		// For a case-insensitive comparison
		RuleFor(x => x.ErrorLevelName).IsEnumName(typeof(ErrorLevel), caseSensitive: false);
		//确保值不在1，10范围内
		RuleFor(x => x.Id).ExclusiveBetween(1,10);
		//确保值在1，10范围内
		RuleFor(x => x.Id).InclusiveBetween(1,10);
		//确保decimal 在指定精度范围内。4total 2小数点
		RuleFor(x => x.CustomerDiscount).PrecisionScale(4, 2, false);

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
	public string Photo { get; set; }
	public int CreditLimit {get;set;}
	public int MaxCreditLimit {get;set;}
	public string Email {get;set;}
	public ErrorLevel ErrorLevel { get; set; }
	public string ErrorLevelName {get;set;}
}

public enum ErrorLevel
{
	Error = 1,
	Warning = 2,
	Notice = 3
}
