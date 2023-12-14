<Query Kind="Statements">
  <NuGetReference Version="6.0.25">Microsoft.EntityFrameworkCore.Sqlite</NuGetReference>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
</Query>


//默认约定 一个属性 Id 或者 <type name>Id 被设置成主键。

public class Car
{
	public string Id { get; set; }

	public string Make { get; set; }
	public string Model { get; set; }
	public string LicensePlate { get; set; }
}

public class Truck
{
	public string TruckId { get; set; }

	public string Make { get; set; }
	public string Model { get; set; }
}


//你也可以使用[Key] 设置主键

//你也可以使用FluentAPI设置主键

public class MyContext : DbContext
{
	public DbSet<Car> Cars { get; set; }

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
		modelBuilder.Entity<Car>(b =>
		{
			 modelBuilder.Entity<Car>()
			.HasKey(c => c.LicensePlate);
		});
	}
}
