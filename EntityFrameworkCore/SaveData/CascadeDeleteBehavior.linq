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

//
//* DeleteBehavior.ClientCascade 由EF来实现级联删除，如果子实体没有加载，抛出异常
//* DeleteBehavior.Cascade 由数据级联删除 在加载子实体时是EF删除， 但是没有加载时数据库删除
//* 

#load ".\Entities"

var connection = CreateDatabaseAndGetConnection();

var contextOptions = new DbContextOptionsBuilder<MyContext>().UseSqlite(connection).Options;

using(var context = new MyContext(contextOptions))
{
	var blog = new Blog 
	{
		Url = "http://blogs.msdn.com/dotnet",
		Posts =
		{
			new Post { Title = "Intro to C#" },
			new Post { Title = "Intro to VB.NET" },
			new Post { Title = "Intro to F#" }
		}
	};
	context.Blogs.Add(blog);
	context.SaveChanges();
	context.Blogs.ToList().Dump();
}

using (var context = new MyContext(contextOptions)) 
{
	var blog = context.Blogs.First();
	context.Remove(blog);
	context.SaveChanges();
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

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder
		.LogTo(Console.WriteLine, LogLevel.Information);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Blog>()
			.Property(b => b.Url)
			.IsRequired();

		modelBuilder.Entity<Blog>()
			.HasMany(b => b.Posts)
			.WithOne(p => p.Blog)
			.HasForeignKey(p => p.BlogId)
			.IsRequired()
			.OnDelete(DeleteBehavior.ClientCascade);
	}
}