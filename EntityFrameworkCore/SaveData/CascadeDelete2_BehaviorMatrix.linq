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

// 级联删除（2/4）：7 种 DeleteBehavior 的行为矩阵
//
// 把 7 种 DeleteBehavior × {必需关系, 可选关系} × {子实体已跟踪, 子实体未加载} 全部跑一遍，
// 删除父实体并 SaveChanges，记录结果。下面的表格是运行结果的预期，以实际输出为准。
//
// 特征 3 —— 7 种行为在 EF 端（子实体已跟踪时）其实只分三类：
//   ① Cascade / ClientCascade                         → 子实体标记为 Deleted
//   ② SetNull / ClientSetNull / Restrict / NoAction   → 子实体外键置 null（关系必需时这一步注定失败，见下）
//   ③ ClientNoAction                                  → EF 什么都不做，直接发 DELETE 给数据库，由数据库报错
//
// 数据库端（子实体未加载时）看建表 SQL 的 ON DELETE 子句：
//   Cascade → ON DELETE CASCADE      SetNull → ON DELETE SET NULL     Restrict → ON DELETE RESTRICT
//   其余（Client* / NoAction）      → 没有 ON DELETE 子句，即 NO ACTION，删父实体时数据库报外键错误
//
// 实际运行结果（本文件的 Post.BlogId 是 int?）：
//   ┌─────────────────┬────────────────────────────────┬────────────────────────────────┐
//   │ DeleteBehavior  │ 必需关系   已跟踪 / 未加载       │ 可选关系   已跟踪 / 未加载       │
//   ├─────────────────┼────────────────────────────────┼────────────────────────────────┤
//   │ Cascade         │ 删除       / 删除（数据库）      │ 删除       / 删除（数据库）      │
//   │ ClientCascade   │ 删除       / ✗ FK 约束           │ 删除       / ✗ FK 约束           │
//   │ SetNull         │ ✗ NOT NULL / ✗ NOT NULL（数据库）│ 置 null    / 置 null（数据库）   │
//   │ ClientSetNull   │ ✗ NOT NULL / ✗ FK 约束           │ 置 null    / ✗ FK 约束           │
//   │ Restrict        │ ✗ NOT NULL / ✗ FK 约束           │ 置 null    / ✗ FK 约束           │
//   │ NoAction        │ ✗ NOT NULL / ✗ FK 约束           │ 置 null    / ✗ FK 约束           │
//   │ ClientNoAction  │ ✗ FK 约束  / ✗ FK 约束           │ ✗ FK 约束  / ✗ FK 约束           │
//   └─────────────────┴────────────────────────────────┴────────────────────────────────┘
//   ✗ FK 约束  = DbUpdateException → SqliteException "FOREIGN KEY constraint failed"（数据库拒绝删父行）
//   ✗ NOT NULL = DbUpdateException → SqliteException "NOT NULL constraint failed: Posts.BlogId"
//                （EF 端把 BlogId 置成了 null 并发 UPDATE，数据库拒绝）
//
//   （若外键的 CLR 类型是不可空的 int，EF 存不了 null，会改为在 ctx.Remove() 时抛 InvalidOperationException，
//     根因相同：非级联行为要求外键可空。示例见 CascadeDelete3_Exceptions 异常2。）

#load ".\CascadeContext"

var rows = new List<object>();

foreach (var required in new[] { true, false })
foreach (var behavior in Enum.GetValues<DeleteBehavior>())
foreach (var tracked in new[] { true, false })
{
	var db = Cascade.CreateDatabase(behavior, required, log: false);
	var blogId = db.Seed();

	var error = Cascade.Run(() =>
	{
		using var ctx = db.NewContext();
		var blog = tracked
			? ctx.Blogs.Include(b => b.Posts).Single(b => b.Id == blogId)   // 子实体已跟踪 → 走 EF 端
			: ctx.Blogs.Find(blogId);                                       // 子实体未加载 → 走数据库端
		ctx.Remove(blog);
		ctx.SaveChanges();
	});

	string outcome;
	if (error != null)
	{
		outcome = "✗ " + error.GetType().Name;
	}
	else
	{
		var posts = db.QueryPosts();
		outcome = posts.Count == 0 ? "子实体已删除"
			: posts.All(p => p.BlogId == null) ? "子实体保留，外键置 null"
			: "?? 子实体保留且外键未变（不应出现）";
	}

	rows.Add(new
	{
		关系 = required ? "必需" : "可选",
		DeleteBehavior = behavior.ToString(),
		子实体 = tracked ? "已跟踪（EF 端）" : "未加载（数据库端）",
		建表ON_DELETE = db.ForeignKeyDdl.Contains("ON DELETE")
			? db.ForeignKeyDdl.Substring(db.ForeignKeyDdl.IndexOf("ON DELETE"))
			: "（无，NO ACTION）",
		结果 = outcome,
		异常消息 = error == null ? "" : Cascade.Describe(error),
	});
}

rows.Dump("DeleteBehavior × 关系类型 × 子实体是否跟踪");
