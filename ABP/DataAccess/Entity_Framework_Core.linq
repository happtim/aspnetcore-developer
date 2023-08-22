<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.Autofac</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.EntityFrameworkCore.Sqlite</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.SettingManagement.EntityFrameworkCore</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.Autofac</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.Settings</Namespace>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Storage</Namespace>
  <Namespace>Volo.Abp.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Infrastructure</Namespace>
  <Namespace>Volo.Abp.Domain.Entities.Auditing</Namespace>
  <Namespace>Volo.Abp.EntityFrameworkCore.Modeling</Namespace>
  <Namespace>Volo.Abp.Data</Namespace>
  <Namespace>Volo.Abp.DependencyInjection</Namespace>
  <Namespace>Volo.Abp.Domain.Repositories</Namespace>
  <Namespace>Volo.Abp.EntityFrameworkCore.Sqlite</Namespace>
</Query>

#load ".\BookStoreContext"

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
		var bookRepository =  context.ServiceProvider.GetRequiredService<IRepository<Book,Guid>>();
		var authorRepository = context.ServiceProvider.GetRequiredService<IRepository<Author,Guid>>();

		var orwell = await authorRepository.InsertAsync(
			new Author()
			{
				Name = "George Orwell",
				BirthDate = new DateTime(1903, 06, 25),
				ShortBio = "Orwell produced literary criticism and poetry, fiction and polemical journalism; and is best known for the allegorical novella Animal Farm (1945) and the dystopian novel Nineteen Eighty-Four (1949)."
			}
		);
		
		await bookRepository.InsertAsync(
			new Book
			{
				AuthorId = orwell.Id, // SET THE AUTHOR
				Name = "1984",
				Type = Book.BookType.Dystopia,
				PublishDate = new DateTime(1949, 6, 8),
				Price = 19.84f
			},
			autoSave: true
		);

		await bookRepository.GetListAsync().Dump();
	}


	public override void OnApplicationShutdown(ApplicationShutdownContext context)
	{
		_sqliteConnection.Dispose();
	}

}


