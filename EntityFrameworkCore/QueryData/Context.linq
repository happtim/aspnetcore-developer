<Query Kind="Statements">
  <NuGetReference Version="6.0.25">Microsoft.EntityFrameworkCore.Sqlite</NuGetReference>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Infrastructure</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Storage</Namespace>
</Query>

// 共享的实体类、DbContext 及数据库创建辅助方法
// 在其他 .linq 文件中通过 #load ".\Context" 引用

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
	public DbSet<Post> Posts { get; set; }

	public MyContext(DbContextOptions<MyContext> options) : base(options) { }

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

public class Blog
{
	public int Id { get; set; }
	public string Url { get; set; }
	public IList<Post> Posts { get; } = new List<Post>();
}

public class Post
{
	public int Id { get; set; }
	public string Title { get; set; }
	public string Content { get; set; }
	public int BlogId { get; set; }
	public Blog Blog { get; set; }
}
