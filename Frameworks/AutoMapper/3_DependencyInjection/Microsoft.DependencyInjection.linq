<Query Kind="Statements">
  <NuGetReference Version="12.0.1">AutoMapper.Extensions.Microsoft.DependencyInjection</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>AutoMapper</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

//Dependency Injection （依赖注入）

//AutoMapper.Extensions.Microsoft.DependencyInjection 这个Nuget包可以使用默认ASP。Net Core 以来注入功能。

this.GetType().Dump();

// Create a new instance of the service collection
var serviceCollection = new ServiceCollection();

//Type
//serviceCollection.AddAutoMapper(typeof(UserQuery));

//or Assembly
serviceCollection.AddAutoMapper(typeof(UserQuery).Assembly);

// Build the service provider and get an instance of the service
var serviceProvider = serviceCollection.BuildServiceProvider();

var mapper =  serviceProvider.GetRequiredService<IMapper>();

var customer = new Customer(Guid.NewGuid(), "Google");

var customerDto = mapper.Map<CustomerDto>(customer);

customerDto.Dump();


public class Customer
{
	public Guid Id { get; }
	public string Name { get; }

	public Customer(Guid id, string name)
	{
		Id = id;
		Name = name;
	}
}

public class CustomerDto
{
	public Guid Id { get; }
	public string Name { get; }

	public CustomerDto(Guid id, string name)
	{
		Id = id;
		Name = name;
	}
}

public class CustomerProfile : Profile
{
	public CustomerProfile()
	{
		CreateMap<Customer, CustomerDto>()
			.ConstructUsing(user => new CustomerDto(user.Id, user.Name))
			.ReverseMap()
			.ConstructUsing(userDto => new Customer(userDto.Id, userDto.Name));
	}
}