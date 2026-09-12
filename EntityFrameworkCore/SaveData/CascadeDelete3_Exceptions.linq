<Query Kind="Statements">
  <NuGetReference Version="6.0.25">Microsoft.EntityFrameworkCore.Sqlite</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.ChangeTracking</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Infrastructure</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Storage</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
</Query>

// 级联删除（3/4）：常见异常示例 —— 每个异常都可复现，并给出对照写法与解决办法
//
// 删除父实体时只会遇到两种异常，看异常类型就能定位是哪一层出的问题：
//   InvalidOperationException  → EF 端：关系被切断但外键不可空，EF 在发 SQL 之前就放弃（见 4_Orphans 场景5）
//                                 消息关键字 "has been severed, but the relationship is either marked as required..."
//   DbUpdateException          → 数据库端：EF 把 SQL 发出去了，数据库约束拒绝
//                                 InnerException 是 SqliteException "FOREIGN KEY constraint failed"
//                                 或 "NOT NULL constraint failed"
//
// 注意：Microsoft.Data.Sqlite 默认开启外键约束（PRAGMA foreign_keys = 1），所以能看到数据库端错误。

#load ".\CascadeContext"

// ====================================================================
// 异常1：ClientCascade + 子实体没有加载 → DbUpdateException
//   ClientCascade 只在 EF 端级联，建表 SQL 没有 ON DELETE 子句。
//   Post 没有进 context，EF 只发 DELETE FROM Blogs，数据库发现 Posts 还引用它 → 拒绝。
//   解决：删之前 Include 子实体（让 EF 端级联），或改用 Cascade（让数据库级联）。
// ====================================================================
{
	var db = Cascade.CreateDatabase(DeleteBehavior.ClientCascade, required: true);
	db.ForeignKeyDdl.Dump("异常1 建表外键约束（没有 ON DELETE）");

	var blogId = db.Seed();
	Cascade.Try(() =>
	{
		using var ctx = db.NewContext();
		var blog = ctx.Blogs.Find(blogId);   // 没有 Include
		ctx.Remove(blog);
		ctx.SaveChanges();
	}, "异常1 ClientCascade + 子实体未加载");

	// 对照：Include 之后再删，EF 端级联，成功
	Cascade.Try(() =>
	{
		using var ctx = db.NewContext();
		var blog = ctx.Blogs.Include(b => b.Posts).Single(b => b.Id == blogId);
		ctx.Remove(blog);
		ctx.SaveChanges();
	}, "异常1 对照：同样是 ClientCascade，先 Include 再删 → 成功");
}

// ====================================================================
// 异常2：必需关系 + ClientSetNull（Restrict / NoAction / SetNull 同理）+ 子实体已跟踪 → DbUpdateException
//   这类行为在 EF 端的动作是"把子实体外键置 null"，但关系是必需的，外键不能为 null。
//   EF 把 Post 标记为 Modified、BlogId 置 null，发 UPDATE Posts SET BlogId = NULL → 数据库 NOT NULL 约束拒绝。
//   解决：要么关系改成可选（IsRequired(false)），要么行为改成 Cascade / ClientCascade。
// ====================================================================
{
	var db = Cascade.CreateDatabase(DeleteBehavior.ClientSetNull, required: true);
	var blogId = db.Seed();

	Cascade.Try(() =>
	{
		using var ctx = db.NewContext();
		var blog = ctx.Blogs.Include(b => b.Posts).Single(b => b.Id == blogId);
		ctx.Remove(blog);
		ctx.DumpPostStates("异常2 [Remove 之后] Post 变为 Modified，BlogId 已被置 null");
		ctx.SaveChanges();
	}, "异常2 必需关系 + ClientSetNull + 子实体已跟踪");

	// Restrict 在 EF 端与 ClientSetNull 完全一样
	var dbRestrict = Cascade.CreateDatabase(DeleteBehavior.Restrict, required: true);
	var blogId2 = dbRestrict.Seed();
	Cascade.Try(() =>
	{
		using var ctx = dbRestrict.NewContext();
		var blog = ctx.Blogs.Include(b => b.Posts).Single(b => b.Id == blogId2);
		ctx.Remove(blog);
		ctx.SaveChanges();
	}, "异常2 同类：必需关系 + Restrict + 子实体已跟踪");
}

// ====================================================================
// 异常3：可选关系 + ClientSetNull（可选关系的默认行为）+ 子实体没有加载 → DbUpdateException
//   EF 端能置 null 的前提是子实体在 context 里；没加载时只发 DELETE FROM Blogs，数据库拒绝。
//   解决：改用 SetNull —— 建表 SQL 带 ON DELETE SET NULL，数据库自己把外键置 null。
// ====================================================================
{
	var db = Cascade.CreateDatabase(behavior: null, required: false);   // 默认 ClientSetNull
	new { db.FkDeleteBehavior, db.ForeignKeyDdl }.Dump("异常3 可选关系默认配置");

	var blogId = db.Seed();
	Cascade.Try(() =>
	{
		using var ctx = db.NewContext();
		var blog = ctx.Blogs.Find(blogId);
		ctx.Remove(blog);
		ctx.SaveChanges();
	}, "异常3 可选关系 + ClientSetNull（默认）+ 子实体未加载");

	// 对照：SetNull → 数据库端 ON DELETE SET NULL，不加载子实体也能成功
	var dbSetNull = Cascade.CreateDatabase(DeleteBehavior.SetNull, required: false);
	dbSetNull.ForeignKeyDdl.Dump("异常3 对照：SetNull 的建表外键约束");
	var blogId2 = dbSetNull.Seed();
	Cascade.Try(() =>
	{
		using var ctx = dbSetNull.NewContext();
		var blog = ctx.Blogs.Find(blogId2);
		ctx.Remove(blog);
		ctx.SaveChanges();
	}, "异常3 对照：SetNull + 子实体未加载 → 成功");
	dbSetNull.QueryPosts().Dump("异常3 对照：数据库里 Post 的 BlogId 已被置 null");
}

// ====================================================================
// 异常4：ClientNoAction + 子实体已跟踪 → DbUpdateException
//   ClientNoAction 是唯一一个 EF 端"完全不管"的行为：子实体即使在 context 里，
//   EF 也不改它的状态和外键，直接发 DELETE FROM Blogs，交给数据库 → 数据库拒绝。
//   用途：自己在 SaveChanges 之前手动处理子实体，或数据库里有 EF 不知道的触发器。
// ====================================================================
{
	var db = Cascade.CreateDatabase(DeleteBehavior.ClientNoAction, required: true);
	var blogId = db.Seed();

	Cascade.Try(() =>
	{
		using var ctx = db.NewContext();
		var blog = ctx.Blogs.Include(b => b.Posts).Single(b => b.Id == blogId);
		ctx.Remove(blog);
		ctx.DumpPostStates("异常4 [Remove 之后] Post 仍是 Unchanged，EF 不做任何处理");
		ctx.SaveChanges();
	}, "异常4 ClientNoAction + 子实体已跟踪");

	// 对照：自己先删子实体，再删父实体
	Cascade.Try(() =>
	{
		using var ctx = db.NewContext();
		var blog = ctx.Blogs.Include(b => b.Posts).Single(b => b.Id == blogId);
		ctx.RemoveRange(blog.Posts);
		ctx.Remove(blog);
		ctx.SaveChanges();
	}, "异常4 对照：ClientNoAction 下手动 RemoveRange(blog.Posts) → 成功");
}
