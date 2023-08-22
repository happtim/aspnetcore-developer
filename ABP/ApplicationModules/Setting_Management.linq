<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.Autofac</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Core</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.EntityFrameworkCore.Sqlite</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.SettingManagement.EntityFrameworkCore</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.Autofac</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.Settings</Namespace>
  <Namespace>Volo.Abp.EntityFrameworkCore.Sqlite</Namespace>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>Volo.Abp.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Storage</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Infrastructure</Namespace>
  <Namespace>Volo.Abp.SettingManagement</Namespace>
</Query>

#load ".\SettingManagementDbContext"

// 1: Create the ABP application container
using var application = await AbpApplicationFactory.CreateAsync<DemoModule>(options =>
{
	options.UseAutofac();
});

// 2: Initialize/start the ABP Framework (and all the modules)
await application.InitializeAsync();

Console.WriteLine("ABP Framework has been started...");

// 3: Stop the ABP Framework (and all the modules)
await application.ShutdownAsync();

[DependsOn(
	typeof(AbpSettingManagementEntityFrameworkCoreModule),
	typeof(AbpAutofacModule),
	typeof(AbpEntityFrameworkCoreSqliteModule))]
public class DemoModule : AbpModule
{
	private SqliteConnection _sqliteConnection;

	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		ConfigureInMemorySqlite(context.Services);
		
		Console.WriteLine("DemoModule Configuring");
	}

	private void ConfigureInMemorySqlite(IServiceCollection services)
	{
		_sqliteConnection = CreateDatabaseAndGetConnection();

		services.Configure<AbpDbContextOptions>(options =>
		{
			options.Configure(context =>
			{
				context.DbContextOptions.UseSqlite(_sqliteConnection);
			});
		});

		//Registering DbContext To Dependency Injection
		services.AddAbpDbContext<SettingManagementDbContext>(options =>
		{
			/* Remove "includeAllEntities: true" to create
			* default repositories only for aggregate roots */
			options.AddDefaultRepositories(includeAllEntities: true);
		});
	}


	private static SqliteConnection CreateDatabaseAndGetConnection()
	{
		var connection = new SqliteConnection("Data Source=:memory:");
		connection.Open();

		var options = new DbContextOptionsBuilder<SettingManagementDbContext>()
			.UseSqlite(connection)
			.Options;

		using (var context = new SettingManagementDbContext(options))
		{
			context.GetService<IRelationalDatabaseCreator>().CreateTables();
		}

		return connection;
	}

	public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
	{
		//获取SettingDefinition
		var manager = context.ServiceProvider.GetRequiredService<ISettingDefinitionManager>();
		manager.Get("Smtp.Host").Dump();

		//获取Value，ISettingProvider只能获取value,不能设置，没有存储功能，需要使用扩展module增强功能。
		var settingProvider = context.ServiceProvider.GetRequiredService<ISettingProvider>();

		//Get a value as string.
		await settingProvider.GetOrNullAsync("Smtp.UserName").Dump();

		//设置vlaue，ISettingManager 可以读取，或者设置。有缓存功能。有众多扩展方法进行读取设置。
		//ISettingManager 相当于又实现了一次ISettingProvider功能，并且增加设置的功能。在读取方面两者相同。
		var settingManager = context.ServiceProvider.GetRequiredService<ISettingManager>();
		
		await settingManager.GetOrNullGlobalAsync("Smtp.UserName").Dump();

		await settingManager.SetGlobalAsync("Smtp.UserName", "Tim");
		
		await settingManager.GetOrNullGlobalAsync("Smtp.UserName").Dump();

		await settingProvider.GetOrNullAsync("Smtp.UserName").Dump();
	}

}

public class EmailSettingProvider : SettingDefinitionProvider
{
	public override void Define(ISettingDefinitionContext context)
	{
		context.Add(
			new SettingDefinition("Smtp.Host", "127.0.0.1"),
			new SettingDefinition("Smtp.Port", "25"),
			new SettingDefinition("Smtp.UserName"),
			new SettingDefinition("Smtp.Password", isEncrypted: true),
			new SettingDefinition("Smtp.EnableSsl", "false")
		);
	}
}