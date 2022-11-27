<Query Kind="Statements">
  <NuGetReference>FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>FluentValidation.Results</Namespace>
</Query>


Customer customer = new Customer() ;
CustomerValidator validator = new CustomerValidator();
//使用Validate方法来运行validator
ValidationResult result = validator.Validate(customer);
//返回ValidationResult对象包含： 
//IsValid 是否验证成功
//Errors 包含验证失败的详细信息。
result.Dump();

if (!result.IsValid)
{
	foreach (var failure in result.Errors)
	{
		Console.WriteLine("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
	}
}

//还可以直接使用result的toString方法，获取所有的错误信息。
result.ToString().Dump();

//可以通过抛出异常的验证方式
try
{
	//ValidateAndThrow 为扩展方法
	//validator.ValidateAndThrow(customer);
	//等于下面方法
	validator.Validate(customer, options =>
		{ 
			options.ThrowOnFailures();
			options.IncludeRuleSets("MyRuleSets"); //可以使用RuleSets
			options.IncludeProperties(x => x.Forename); //可以指定字段。
		}
	);
}
catch(Exception ex)
{
	ex.Dump();
}


//通过继承AbstractValidator<>来编写规则
public class CustomerValidator : AbstractValidator<Customer>
{
	//在构造函数中编写规则
	public CustomerValidator()
	{
		//通过使用RuleFor的lambda表达式 指定验证规则
		RuleFor(customer => customer.Surname).NotNull().NotEqual("foo");
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