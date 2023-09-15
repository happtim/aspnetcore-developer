<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.AutoMapper</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Core</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.ObjectMapping</Namespace>
  <Namespace>Volo.Abp.AutoMapper</Namespace>
  <Namespace>AutoMapper</Namespace>
</Query>

// 1: Create the ABP application container
using var application = await AbpApplicationFactory.CreateAsync<DomeModule>();

// 2: Initialize/start the ABP Framework (and all the modules)
await application.InitializeAsync();

Console.WriteLine("ABP Framework has been started...");

// 3: Stop the ABP Framework (and all the modules)
await application.ShutdownAsync();

[DependsOn(
	typeof(AbpAutoMapperModule),
	typeof(Dome1Module)
)]
public class DomeModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Console.WriteLine("Dome Configuring");

		Configure<AbpAutoMapperOptions>(options =>
		{
			//Add all mappings defined in the assembly of the MyModule class
			//options.AddMaps<DomeModule>();
			
			options.AddProfile<MyProfile>();
		});
	}

	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		var input = new CreateUserInput{ Name="Tim",Surname="Ge",EmailAddress="bob@example.com",Password="Pass123$"};
		var user = context.ServiceProvider.GetRequiredService<IObjectMapper>().Map<CreateUserInput,User>(input);
		user.Dump();
		
		var userDto = context.ServiceProvider.GetRequiredService<IObjectMapper<Dome1Module>>().Map<CreateUserInput,UserDto>(input);
		userDto.Dump();
		Console.WriteLine("Dome Initializing");
	}

	public override void OnApplicationShutdown(ApplicationShutdownContext context)
	{
		Console.WriteLine("Dome Shutdown");
	}
}

[DependsOn(typeof(AbpAutoMapperModule))]
public class Dome1Module : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Console.WriteLine("Dome1 Configuring");

		//考虑到模块化，终端应用可能自己Mapper。所以模块内使用IObjectMapper<Dome1Module>。
		//需要AddAutoMapperObjectMapper<Dome1Module>();
		//Use AutoMapper for MyModule
		context.Services.AddAutoMapperObjectMapper<Dome1Module>();
		
		Configure<AbpAutoMapperOptions>(options =>
		{
			//Add all mappings defined in the assembly of the MyModule class
			//options.AddMaps<DomeModule>();

			options.AddProfile<MyProfile1>();
		});
	}
}

public class MyProfile1 : Profile
{
	public MyProfile1()
	{
		CreateMap<CreateUserInput, UserDto>().Ignore(u => u.Name);
	}
}


public class MyProfile : Profile
{
	public MyProfile()
	{
		CreateMap<CreateUserInput,User>().Ignore(u => u.Surname);
	}
}

class UserDto : User { }

class CreateUserInput : User { }

class User 
{
	public string Name {get;set;}
    public string Surname {get;set;}
    public string EmailAddress {get;set;}
    public string Password {get;set;}
}