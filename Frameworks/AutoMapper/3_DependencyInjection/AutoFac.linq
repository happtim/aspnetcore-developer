<Query Kind="Statements">
  <NuGetReference Version="7.1.0">AutoMapper.Contrib.Autofac.DependencyInjection</NuGetReference>
  <Namespace>Autofac</Namespace>
  <Namespace>AutoMapper</Namespace>
  <Namespace>AutoMapper.Contrib.Autofac.DependencyInjection</Namespace>
</Query>


//AutoMapper.Contrib.Autofac.DependencyInjection 该包实现Autofact 用来注入

//使用AutoFac注入
var containerBuilder = new ContainerBuilder();
// here you have to pass in the assembly (or assemblies) containing AutoMapper types
// stuff like profiles, resolvers and type-converters will be added by this function
containerBuilder.RegisterAutoMapper(typeof(UserQuery).Assembly);

var container = containerBuilder.Build();

var mapper = container.Resolve<IMapper>();

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
