<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.EntityFrameworkCore.Sqlite</NuGetReference>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Volo.Abp.EntityFrameworkCore.Modeling</Namespace>
  <Namespace>Volo.Abp.Domain.Entities.Auditing</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Storage</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Infrastructure</Namespace>
  <Namespace>Volo.Abp.EntityFrameworkCore.Sqlite</Namespace>
</Query>


public class BookStoreDbContext : AbpDbContext<BookStoreDbContext>
{
	public DbSet<Book> Books { get; set; }
	
	public DbSet<Author> Authors { get; set; }

	public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Book>(b =>
	  	{
			  b.ToTable("AppBooks");
			  b.ConfigureByConvention(); //auto configure for the base class props
			  b.Property(x => x.Name).IsRequired().HasMaxLength(128);

			  b.HasOne<Author>().WithMany().HasForeignKey(x => x.AuthorId).IsRequired();
		  });

		modelBuilder.Entity<Author>(b =>
		 {
			 b.ToTable( "AppAuthors");

			 b.ConfigureByConvention();

			 b.Property(x => x.Name)
				 .IsRequired()
				 .HasMaxLength(64);

			 b.HasIndex(x => x.Name);
		 });
	}
}

public class Book : AuditedAggregateRoot<Guid>
{
	public Guid AuthorId { get; set; }

	public string Name { get; set; }
	
	public BookType Type { get; set; }

	public DateTime? PublishDate { get; set; }

	public float Price { get; set; }

	public enum BookType
	{
		Undefined,
		Adventure,
		Biography,
		Dystopia,
		Fantastic,
		Horror,
		Science,
		ScienceFiction,
		Poetry
	}
}



public class Author : FullAuditedAggregateRoot<Guid>
{
	public Author()
	{
		Id = Guid.NewGuid();
	}
	public string Name { get;  set; }
	public DateTime? BirthDate { get; set; }
	public string ShortBio { get; set; }
}