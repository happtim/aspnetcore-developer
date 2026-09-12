<Query Kind="Statements">
  <NuGetReference Version="6.0.25">Microsoft.EntityFrameworkCore.Sqlite</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.ChangeTracking</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Diagnostics</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Infrastructure</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Storage</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
</Query>

// ====================================================================
// 级联删除系列脚本（CascadeDelete1~5）的共享代码，本文件不直接运行。
//
// 与 Entities.linq 的区别：这里 Post.BlogId 是 int?，
// 关系是"必需"还是"可选"由 IsRequired(bool) 决定，这样同一套实体既能演示级联删除，也能演示 SetNull。
//
// 每个场景都通过 Cascade.CreateDatabase(behavior, required) 建一个独立的内存 SQLite 库，
// 互不干扰；DeleteBehavior 传 null 表示"不配置 OnDelete，走 EF 默认约定"。
// ====================================================================

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
	public int? BlogId { get; set; }   // CLR 可空；模型层面是否必需由 IsRequired 决定
	public Blog Blog { get; set; }
}

public class CascadeContext : DbContext
{
	public DeleteBehavior? Behavior { get; }
	public bool Required { get; }
	public bool Log { get; }

	public DbSet<Blog> Blogs { get; set; }
	public DbSet<Post> Posts { get; set; }

	public CascadeContext(DbContextOptions<CascadeContext> options, DeleteBehavior? behavior, bool required, bool log = true)
		: base(options)
	{
		Behavior = behavior;
		Required = required;
		Log = log;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (Log)
		{
			// 只打印执行的 SQL，便于观察 EF 到底发了几条 DELETE / UPDATE
			optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Blog>().Property(b => b.Url).IsRequired();

		var relationship = modelBuilder.Entity<Blog>()
			.HasMany(b => b.Posts)
			.WithOne(p => p.Blog)
			.HasForeignKey(p => p.BlogId)
			.IsRequired(Required);

		if (Behavior != null)
		{
			relationship.OnDelete(Behavior.Value);
		}
	}

	/// <summary>Post -> Blog 这条外键在模型中的 DeleteBehavior（不配置时即为 EF 的默认约定）</summary>
	public DeleteBehavior FkDeleteBehavior
		=> Model.FindEntityType(typeof(Post)).GetForeignKeys().Single().DeleteBehavior;
}

/// <summary>
/// EF 默认按 DbContext 类型缓存模型。本示例同一个 CascadeContext 类型要用不同的
/// DeleteBehavior / Required 构建不同模型，所以把这两个参数也纳入缓存键。
/// </summary>
public class CascadeModelCacheKeyFactory : IModelCacheKeyFactory
{
	public object Create(DbContext context, bool designTime)
		=> context is CascadeContext c
			? (context.GetType(), c.Behavior, c.Required, designTime)
			: (object)(context.GetType(), designTime);
}

/// <summary>一个已建好表的内存数据库 + 它对应的模型配置</summary>
public class CascadeDb
{
	public DbContextOptions<CascadeContext> Options { get; init; }
	public DeleteBehavior? Behavior { get; init; }
	public bool Required { get; init; }
	public bool Log { get; init; }

	/// <summary>模型中实际生效的 DeleteBehavior</summary>
	public DeleteBehavior FkDeleteBehavior { get; init; }
	/// <summary>建表脚本里 Posts 表的外键约束那一行，可看到 ON DELETE ... 子句</summary>
	public string ForeignKeyDdl { get; init; }

	public CascadeContext NewContext() => new CascadeContext(Options, Behavior, Required, Log);
}

public static class Cascade
{
	/// <summary>
	/// 新建一个内存 SQLite 库并按给定配置建表。
	/// behavior = null 表示不调用 OnDelete，观察 EF 默认约定。
	/// </summary>
	public static CascadeDb CreateDatabase(DeleteBehavior? behavior, bool required, bool log = true)
	{
		// 内存库的生命周期 = 连接的生命周期，连接保持打开，不关闭
		var connection = new SqliteConnection("Data Source=:memory:");
		connection.Open();

		var options = new DbContextOptionsBuilder<CascadeContext>()
			.UseSqlite(connection)
			.ReplaceService<IModelCacheKeyFactory, CascadeModelCacheKeyFactory>()
			.Options;

		using var ctx = new CascadeContext(options, behavior, required, log: false);
		ctx.GetService<IRelationalDatabaseCreator>().CreateTables();

		var ddl = ctx.Database.GenerateCreateScript();
		var fkLine = ddl
			.Split('\n')
			.Select(l => l.Trim())
			.FirstOrDefault(l => l.StartsWith("CONSTRAINT") && l.Contains("FOREIGN KEY")) ?? "(无外键约束)";

		return new CascadeDb
		{
			Options = options,
			Behavior = behavior,
			Required = required,
			Log = log,
			FkDeleteBehavior = ctx.FkDeleteBehavior,
			ForeignKeyDdl = fkLine.TrimEnd(','),
		};
	}

	/// <summary>插入 1 个 Blog + 3 个 Post，返回 Blog.Id</summary>
	public static int Seed(this CascadeDb db)
	{
		using var ctx = new CascadeContext(db.Options, db.Behavior, db.Required, log: false);
		var blog = new Blog
		{
			Url = "http://blogs.msdn.com/dotnet",
			Posts =
			{
				new Post { Title = "Intro to C#" },
				new Post { Title = "Intro to VB.NET" },
				new Post { Title = "Intro to F#" },
			}
		};
		ctx.Blogs.Add(blog);
		ctx.SaveChanges();
		return blog.Id;
	}

	/// <summary>用一个新的（不打日志的）context 查询数据库里 Posts 表的实际内容</summary>
	public static List<Post> QueryPosts(this CascadeDb db)
	{
		using var ctx = new CascadeContext(db.Options, db.Behavior, db.Required, log: false);
		return ctx.Posts.AsNoTracking().OrderBy(p => p.Id).ToList();
	}

	/// <summary>Dump 当前 context 里所有被跟踪的 Post 的 Id / BlogId / State</summary>
	public static void DumpPostStates(this CascadeContext ctx, string title)
	{
		ctx.ChangeTracker.Entries<Post>()
			.Select(e => new { e.Entity.Id, e.Entity.Title, e.Entity.BlogId, e.State })
			.OrderBy(x => x.Id)
			.ToList()
			.Dump(title);
	}

	/// <summary>执行 action，成功返回 null，失败返回异常（不抛出）</summary>
	public static Exception Run(Action action)
	{
		try
		{
			action();
			return null;
		}
		catch (Exception ex)
		{
			return ex;
		}
	}

	/// <summary>把异常压成一行：类型 + 消息（含 InnerException）</summary>
	public static string Describe(Exception ex)
	{
		if (ex == null) return "成功";
		var text = $"{ex.GetType().Name}: {ex.Message}";
		if (ex.InnerException != null)
		{
			text += $"\n  --> {ex.InnerException.GetType().Name}: {ex.InnerException.Message}";
		}
		return text;
	}

	/// <summary>执行 action 并 Dump 结果（成功 / 异常详情），用于异常示例</summary>
	public static Exception Try(Action action, string title)
	{
		var ex = Run(action);
		Describe(ex).Dump(title);
		return ex;
	}
}
