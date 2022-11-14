<Query Kind="Program">
  <NuGetReference>AutoMapper.Contrib.Autofac.DependencyInjection</NuGetReference>
  <Namespace>Autofac</Namespace>
  <Namespace>AutoMapper</Namespace>
  <Namespace>AutoMapper.Contrib.Autofac.DependencyInjection</Namespace>
</Query>

//Dependency Injection （依赖注入）
// 使用ASP.NET Core
// AutoMapper.Extensions.Microsoft.DependencyInjection 这个Nuget包可以使用默认ASP。Net Core 以来注入功能。
// 
// adds profiles to mapping configuration
// adds value resolvers, member value resolvers, type converters to the container.


// services.AddAutoMapper(assembly1, assembly2 /*, ...*/);



void Main()
{
	//使用AutoFac注入
	var containerBuilder = new ContainerBuilder();
	// here you have to pass in the assembly (or assemblies) containing AutoMapper types
	// stuff like profiles, resolvers and type-converters will be added by this function
	containerBuilder.RegisterAutoMapper(typeof(UserQuery).Assembly);

	var container = containerBuilder.Build();

	var mapper = container.Resolve<IMapper>();

	var customer = new Customer(Guid.NewGuid(), "Google");
	
	customer.Dump();

	var customerDto = mapper.Map<CustomerDto>(customer);
	
	customerDto.Dump();

}



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
