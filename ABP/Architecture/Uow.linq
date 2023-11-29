<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.Autofac</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Core</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.EntityFrameworkCore.Sqlite</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Uow</NuGetReference>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.Domain.Repositories</Namespace>
  <Namespace>Volo.Abp.EntityFrameworkCore</Namespace>
  <Namespace>Volo.Abp.EntityFrameworkCore.Sqlite</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.Uow</Namespace>
  <Namespace>Volo.Abp.Autofac</Namespace>
</Query>

#load "..\DataAccess\BookStoreDbContext"

//对于没有UOW注入的方法，如一些handle，worker，如果时间repository，那么每个仓库就会有个一独立的uow。
//要使得他们用一个uow，入口方法就要加[UninOfWorkd] ，然后必须virtual方法

// 1: Create the ABP application container
using var application = await AbpApplicationFactory.CreateAsync<UowModule>(options =>
{
	options.UseAutofac();
});

// 2: Initialize/start the ABP Framework (and all the modules)
await application.InitializeAsync();

Console.WriteLine("ABP Framework has been started...");

// 3: Stop the ABP Framework (and all the modules)
await application.ShutdownAsync();

[DependsOn(
	typeof(AbpAutofacModule),
	typeof(AbpUnitOfWorkModule),
	typeof(AbpEntityFrameworkCoreSqliteModule)
)]
public class UowModule: AbpModule
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
		services.AddAbpDbContext<BookStoreDbContext>(options =>
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

		var options = new DbContextOptionsBuilder<BookStoreDbContext>()
			.UseSqlite(connection)
			.Options;

		using (var context = new BookStoreDbContext(options))
		{
			context.GetService<IRelationalDatabaseCreator>().CreateTables();
		}

		return connection;
	}

	public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
	{
		IUnitOfWorkManager unitOfWorkManager = context.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
		
		using(var uow1 = unitOfWorkManager.Begin())
		{
			(uow1 == unitOfWorkManager.Current).Dump("当前uow == uow1");

			var authorRepository = unitOfWorkManager.Current.ServiceProvider.GetRequiredService<IRepository<Author, Guid>>();

			var orwell = await authorRepository.InsertAsync(
				new Author()
				{
					Name = "George Orwell",
					BirthDate = new DateTime(1903, 06, 25),
					ShortBio = "Orwell produced literary criticism and poetry, fiction and polemical journalism; and is best known for the allegorical novella Animal Farm (1945) and the dystopian novel Nineteen Eighty-Four (1949)."
				},true
			);

			using (var uow2 = unitOfWorkManager.Begin())
			{
				(uow1 == unitOfWorkManager.Current).Dump("当前uow == uow1");

				var authorRepository2 = unitOfWorkManager.Current.ServiceProvider.GetRequiredService<IRepository<Author, Guid>>();
				var author = await authorRepository2.GetAsync(r => r.Name == "George Orwell");
				author.BirthDate = new DateTime(1903, 06, 26);
				
				await authorRepository2.UpdateAsync(author);
				
				(await authorRepository.GetDbContextAsync() == await authorRepository2.GetDbContextAsync())
				.Dump("resp == resp2");
				
				await uow2.CompleteAsync();
			}
			
			await uow1.CompleteAsync();
		}
	}

	public override void OnApplicationShutdown(ApplicationShutdownContext context)
	{
		_sqliteConnection.Dispose();
	}
}

