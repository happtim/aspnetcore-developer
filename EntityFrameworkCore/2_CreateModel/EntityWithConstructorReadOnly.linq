<Query Kind="Statements">
  <NuGetReference Version="6.0.25">Microsoft.EntityFrameworkCore.Sqlite</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Infrastructure</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Storage</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>


//只读属性
//一旦通过构造函数设置属性，将某些属性设置为只读可能是有意义的。EF Core支持这一点，但需要注意一些事项。

//注意：
//* 没有Setter属性不会映射 （这样会映射到数据库一些不应该值，如计算值）（需要OnModelCreating显示映射）
//* ef 会将private set也视为 public set. 测试：Blog()构造函数解除注释。
//* Id自动生成，需要一个可读写的属性，在插入时设置键。 测试：Id Set去掉报错
//* 使用private set，代替方案就没有Set属性设置，成为真正的只读。并且在OnModelCreating明确添加映射。

var connection = CreateDatabaseAndGetConnection();

using(var context = new MyContext(connection))
{
	context.Blogs.Add(new Blog("123","tim"));
	context.Blogs.Add(new Blog("1234","timge"));
	context.SaveChanges();
}

using (var context = new MyContext(connection))
{
	Console.WriteLine("Begin Query");
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
		optionsBuilder.UseSqlite(_connection);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Blog>(b => 
		{
			b.Property(e => e.Name).IsRequired();
			b.Property(e => e.Author);
		});
	}
}

public class Blog
{
	public int Id { get; private set;}
	public string Name { get; private set; }
	public string Author { get;}

	//protected Blog()
	//{
	//	Console.WriteLine("Blog()");
	//}

	public Blog( string name,string author)
	{
		Name = name;
		Author = author;
		Console.WriteLine("Blog(string,string)");
	}

}