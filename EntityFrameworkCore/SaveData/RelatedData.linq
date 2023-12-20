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

//add
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
	context.Blogs.Include(b => b.Posts).ToList().Dump("Add");
}

//add exits
using(var context = new MyContext(contextOptions))
{
	var blog = 	context.Blogs.Include(b => b.Posts).First();
	var post = new Post { Title = "Intro to EF Core" };
	blog.Posts.Add(post);
	context.SaveChanges();
	
	context.Blogs.Include(b => b.Posts).ToList().Dump("Add Exits");
}

//update
using (var context = new MyContext(contextOptions)) 
{
	var blog = new Blog { Url = "http://blogs.msdn.com/visualstudio" };
	var post = context.Posts.First();

	post.Blog = blog;
	context.SaveChanges();
	
	context.Blogs.Include(b => b.Posts).ToList().Dump("Update");
}

//remove
using (var context = new MyContext(contextOptions)) 
{
	var blog = context.Blogs.Include(b => b.Posts).First();
	var post = blog.Posts.First();
	
	blog.Posts.Remove(post);
	//or
	//blog.Posts.Remove(post);
	context.SaveChanges();
	
	context.Blogs.Include(b => b.Posts).ToList().Dump("Remove");
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
	
	public DbSet<Post> Posts  {get; set; }
	
	public MyContext( DbContextOptions<MyContext> options)
		:base(options)
	{
		
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Blog>()
			.Property(b => b.Url)
			.IsRequired();
		
		modelBuilder.Entity<Blog>()
			.HasMany(b => b.Posts)
			.WithOne(p => p.Blog)
			.HasForeignKey(p => p.BlogId);
	}
}
