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

//如果父实体被删除，子实体的外键将不再与任何父实体主键关联，这个就是无效的状态，在大多数数据库会导致引用约束违规
//解决方法：
//1. 设置子实体外键为空 （要求外键需要nullable）
//2. 同时删除子实体 （级联删除）

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
			.IsRequired();
	}
}