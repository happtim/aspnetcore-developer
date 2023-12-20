<Query Kind="Statements">
  <NuGetReference Version="6.0.25">Microsoft.EntityFrameworkCore.Sqlite</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Infrastructure</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Storage</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
</Query>



var connection = CreateDatabaseAndGetConnection();

using(var context = new MyContext(connection))
{
	context.Blogs.Add(new Blog {Id = 1, Name = "123", Author = "Tim"});
	context.SaveChanges();
	context.Blogs.ToList().Dump();
}

static SqliteConnection CreateDatabaseAndGetConnection()
{
	var connection = new SqliteConnection("Data Source=:memory:");
	connection.Open();

	new MyContext(connection).Database.EnsureCreated();
	
	return connection;
}

public class MyContext : DbContext
{
	public DbSet<Blog> Blogs { get; set; }
	
	private readonly SqliteConnection _connection;
	
	public MyContext(SqliteConnection connection)
	{
		_connection = connection;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
	{
		optionsBuilder
		.LogTo(
			//message => Debug.WriteLine(message), //Logging to Debug
			Console.WriteLine, //Logging to Console
			new[] { DbLoggerCategory.Database.Command.Name }, //Message Categories
			LogLevel.Information //Log Level
			)
		.EnableSensitiveDataLogging() //敏感数据
		.UseSqlite(_connection);
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