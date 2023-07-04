<Query Kind="Statements">
    <NuGetReference Version="11.5.2">FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/collections.html

//使用RuleForEach 应用在集合元素中。
//简单集合
var person = new Person()
{
	AddressLines = new List<string>() { "123", null }
};

PersonValidator validator = new PersonValidator();
var result =  validator.Validate(person);
result.ToString().Dump();

var customer = new Customer() { Orders = new List<Order>() { new Order() {Total = 1 }, new Order() {Cost = "12"}}};
CustomerValidator cvalidator = new CustomerValidator();
result =  cvalidator.Validate(customer);
result.ToString().Dump();

public class PersonValidator : AbstractValidator<Person>
{
	public PersonValidator()
	{
		//可以使用 {CollectionIndex} 获取集合遍历的索引
		RuleForEach(x => x.AddressLines).NotNull().WithMessage("Address {CollectionIndex} is required.");
	}
}

public class Person
{
	public List<string> AddressLines { get; set; } = new List<string>();
}


//复杂集合
public class OrderValidator : AbstractValidator<Order>
{
	public OrderValidator()
	{
		RuleFor(x => x.Total).GreaterThan(0);
	}
}

public class CustomerValidator : AbstractValidator<Customer>
{
	//1.RuleForEach 方式
	public CustomerValidator()
	{
		RuleForEach(x => x.Orders).SetValidator(new OrderValidator());
	}

	//2.使用ChildRules 方式
	//public CustomerValidator()
	//{
	//	RuleForEach(x => x.Orders).ChildRules(orders =>
	//	{
	//		orders.RuleFor(x => x.Total).GreaterThan(0);
	//	});
	//}

	//可以使用Where方法进行筛选，选择可用的子对象进行验证。
	//public CustomerValidator()
	//{
	//	RuleForEach(x => x.Orders)
	//	  .Where(x => x.Cost != null)
	//	  .SetValidator(new OrderValidator());
	//}

	//可以使用ForEach在RuleFor之后来代替RuleForEach。这样就可以通过写一个Fluent表达式验证子对象和集合。
	//但是不建议使用这样方式，还是可以分开的方式表达式更清晰。
	//public CustomerValidator()
	//{
	//	RuleFor(x => x.Orders)
	//	  .Must(x => x.Count <= 10).WithMessage("No more than 10 orders are allowed")
	//	  .ForEach(orderRule => 
	//	  {
	//			orderRule.Must(order => order.Total > 0).WithMessage("Orders must have a total of more than 0");
	//	  });
	//}
	
}

public class Customer
{
	public List<Order> Orders { get; set; } = new List<Order>();
}

public class Order
{
	public double Total { get; set; }
	public string Cost {get;set;}
}