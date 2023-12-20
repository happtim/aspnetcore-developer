<Query Kind="Statements">
  <NuGetReference Version="6.0.25">Microsoft.EntityFrameworkCore.Sqlite</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Infrastructure</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Storage</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
</Query>


var services = new ServiceCollection();

var connection = CreateDatabaseAndGetConnection();

services.AddDbContext<MyContext>(options => options.UseSqlite(connection));

var serviceProvider = services.BuildServiceProvider();

using(var context = serviceProvider.GetRequiredService<MyContext>())
{
	context.Blogs.Add(new Blog {Id = 1, Name = "123", Author = "Tim"});
	context.SaveChanges();
	context.Blogs.ToList().Dump();
}

static SqliteConnection CreateDatabaseAndGetConnection()
{
	var connection = new SqliteConnection("Data Source=:memory:");
	connection.Open();

	new MyContext(
			new DbContextOptionsBuilder<MyContext>().UseSqlite(connection).Options
		).GetService<IRelationalDatabaseCreator>().CreateTables();

	return connection;
}

public class MyContext : DbContext
{
	public DbSet<Blog> Blogs { get; set; }

	public MyContext(DbContextOptions<MyContext> options)
	: base(options)
	{

	}
	
	//静态全局的LoggerFactory，为每个Context唯一Factory
	public static readonly ILoggerFactory MyLoggerFactory =
			LoggerFactory.Create(builder => builder
				.AddFilter(DbLoggerCategory.Database.Command.Name,LogLevel.Information )
				.AddConsole());

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
	{

		optionsBuilder
			//.EnableSensitiveDataLogging() 敏感数据
			.UseLoggerFactory(MyLoggerFactory);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Blog>()
			.Property(b => b.Name)
			.IsRequired();
	}
}

public class Blog
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Author { get; set; }
}