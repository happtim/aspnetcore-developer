<Query Kind="Statements">
  <NuGetReference Version="6.0.25">Microsoft.EntityFrameworkCore.Sqlite</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Infrastructure</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Storage</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>


//优先使用无参构造函数创建Entity
//如果没有无参构造函数，找到有参数的（类型，命名规则符合约定）则用有参数的构造参数。

//注意
//* 有参数构造函数参数可以不必全部包含
//* 命名约定必须是Entity Properties为Pascal-cased 构造函数的参数必须为camel-cased。
//* 不会通过构造函数设置导航属性
//* 构造函数可以public，private，protected

var connection = CreateDatabaseAndGetConnection();

using(var context = new MyContext(connection))
{
	context.Blogs.Add(new Blog(1,"123","Tim"));
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

	protected Blog()
	{
		Console.WriteLine("Blog()");
	}

	public Blog(int id, string name,string author)
	{
		Id = id;
		Name = name;
		Author = author;
		Console.WriteLine("Blog(int,string,string)");
	}

}