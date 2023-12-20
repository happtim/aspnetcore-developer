<Query Kind="Statements">
  <NuGetReference Version="6.0.25">Microsoft.EntityFrameworkCore.Sqlite</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Infrastructure</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Storage</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

#load ".\Entities"

var connection = CreateDatabaseAndGetConnection();

var contextOptions = new DbContextOptionsBuilder<MyContext>().UseSqlite(connection).Options;

using(var context = new MyContext(contextOptions))
{
	//add
	context.Blogs.Add(new Blog { Id = 1, Url = "http://example.com" });
	context.SaveChanges();
	context.Blogs.ToList().Dump();
	
	//update
	var blog = context.Blogs.Single(b =>b.Id == 1 );
	blog.Url = "http://example.com/blog/1";
	context.SaveChanges();
	context.Blogs.ToList().Dump();
	
	//remove
	blog = context.Blogs.Single(b =>b.Id == 1 );
	context.Blogs.Remove(blog);
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
	
	public MyContext( DbContextOptions<MyContext> options)
		:base(options)
	{
		
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Blog>()
			.Property(b => b.Url)
			.IsRequired();
	}
}