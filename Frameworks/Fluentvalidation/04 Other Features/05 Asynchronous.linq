<Query Kind="Statements">
  <NuGetReference>FluentValidation</NuGetReference>
  <Namespace>FluentValidation</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//https://docs.fluentvalidation.net/en/latest/async.html

//有些情况需要定义异步的验证器，异步验证其使用外部的API。
//FluentValidation 提供了MustAsync 和 CustomAsync 使用异步的方法验证，还有条件验证 WhenAsync
var customer = new Customer();
var validator = new CustomerValidator(new SomeExternalWebApiClient());
var result = await validator.ValidateAsync(customer);
result.ToString().Dump();

public class CustomerValidator : AbstractValidator<Customer>
{
	SomeExternalWebApiClient _client;

	public CustomerValidator(SomeExternalWebApiClient client)
	{
		_client = client;

		RuleFor(x => x.Id).MustAsync(async (id, cancellation) =>
		{
			bool exists = await _client.IdExists(id);
			return !exists;
		}).WithMessage("ID Must be unique");
	}
}

public class SomeExternalWebApiClient 
{
	public Task<bool> IdExists(Guid id)
	{
		return Task.FromResult(true);
	}
}


public class Customer
{
	public Guid Id {get;set;}	
}