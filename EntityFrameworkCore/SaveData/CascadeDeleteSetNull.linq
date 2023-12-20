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
	
	
	//删除blog 并设置post.blogid=null
	//context.Remove(blog);
	
	//只设置post.blogid=null
	foreach (var post in blog.Posts)
	{
		post.Blog = null;
	}
	
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
			.IsRequired(false);
	}
}

public class Blog
{
	public int Id { get; set; }

	public string Name { get; set; }

	public string Url { get; set; }

	public IList<Post> Posts { get; } = new List<Post>();
}

public class Post
{
	public int Id { get; set; }

	public string Title { get; set; }
	public string Content { get; set; }

	public int? BlogId { get; set; }
	public Blog Blog { get; set; }
}