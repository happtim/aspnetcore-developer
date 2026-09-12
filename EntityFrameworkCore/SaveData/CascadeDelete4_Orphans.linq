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

// 级联删除（4/4）：孤儿删除（Delete Orphans）—— 不删父实体，只切断关系
//
// 特征 4 —— 级联删除不只在"删父实体"时触发。把子实体从关系里摘出来（severing）也会触发：
//   * blog.Posts.Remove(post) / blog.Posts.Clear()   从集合导航移除
//   * post.Blog = null                                引用导航置 null
//   * post.BlogId = null                              外键置 null
// 被摘出来的子实体成了"孤儿"，EF 按关系的 DeleteBehavior 处理它：
//   必需关系 + Cascade / ClientCascade（必需关系的默认值） → 孤儿被标记为 Deleted，SaveChanges 时 DELETE
//   可选关系                                                → 孤儿保留，外键置 null，SaveChanges 时 UPDATE
//   必需关系 + 其他行为                                     → 失败：外键 int 时抛 InvalidOperationException（关系已切断但必需），
//                                                             外键 int? 时 EF 写 null 被数据库 NOT NULL 约束拒绝（DbUpdateException）
// 触发时机由 ChangeTracker.DeleteOrphansTiming 控制（默认 Immediate，在 DetectChanges 时处理）。
//
// 这也是 RelatedData.linq "remove" 场景里 blog.Posts.Remove(post) 真正删掉一行的原因：
// 那里外键不可空、没配 OnDelete，走的就是"必需关系 + 默认 Cascade → 孤儿删除"。

#load ".\CascadeContext"

// ====================================================================
// 场景1：必需关系（默认 Cascade），从集合移除 → 孤儿 Post 被标记为 Deleted
// ====================================================================
{
	var db = Cascade.CreateDatabase(behavior: null, required: true);
	var blogId = db.Seed();

	using var ctx = db.NewContext();
	var blog = ctx.Blogs.Include(b => b.Posts).Single(b => b.Id == blogId);

	var post = blog.Posts.First();
	blog.Posts.Remove(post);
	ctx.ChangeTracker.DetectChanges();   // SaveChanges / Entries() 也会自动调用，这里显式调用便于观察

	ctx.DumpPostStates("场景1 [Posts.Remove 之后] 被移除的 Post 状态为 Deleted");
	ctx.SaveChanges();                   // 日志：1 条 DELETE FROM Posts
	db.QueryPosts().Dump("场景1 结果：数据库剩 2 个 Post");
}

// ====================================================================
// 场景2：必需关系（默认 Cascade），引用导航置 null → 同样是孤儿删除
// ====================================================================
{
	var db = Cascade.CreateDatabase(behavior: null, required: true);
	var blogId = db.Seed();

	using var ctx = db.NewContext();
	var blog = ctx.Blogs.Include(b => b.Posts).Single(b => b.Id == blogId);

	blog.Posts.First().Blog = null;
	ctx.ChangeTracker.DetectChanges();

	ctx.DumpPostStates("场景2 [post.Blog = null 之后] 该 Post 状态为 Deleted");
	ctx.SaveChanges();
	db.QueryPosts().Dump("场景2 结果：数据库剩 2 个 Post");
}

// ====================================================================
// 场景3：可选关系（默认 ClientSetNull），Posts.Clear() → 孤儿保留，外键置 null
// ====================================================================
{
	var db = Cascade.CreateDatabase(behavior: null, required: false);
	var blogId = db.Seed();

	using var ctx = db.NewContext();
	var blog = ctx.Blogs.Include(b => b.Posts).Single(b => b.Id == blogId);

	blog.Posts.Clear();
	ctx.ChangeTracker.DetectChanges();

	ctx.DumpPostStates("场景3 [Posts.Clear 之后] 3 个 Post 状态为 Modified，BlogId = null");
	ctx.SaveChanges();                   // 日志：3 条 UPDATE Posts SET BlogId = NULL
	db.QueryPosts().Dump("场景3 结果：3 个 Post 都还在，BlogId 为 null");
}

// ====================================================================
// 场景4：可选关系 + 显式 Cascade，Posts.Clear() → 外键本可置 null，但配置了级联，孤儿仍被删除
//   说明孤儿是否删除看的是 DeleteBehavior，而不是外键是否可空
// ====================================================================
{
	var db = Cascade.CreateDatabase(DeleteBehavior.Cascade, required: false);
	var blogId = db.Seed();

	using var ctx = db.NewContext();
	var blog = ctx.Blogs.Include(b => b.Posts).Single(b => b.Id == blogId);

	blog.Posts.Clear();
	ctx.ChangeTracker.DetectChanges();

	ctx.DumpPostStates("场景4 [Posts.Clear 之后] 可选关系 + Cascade，Post 状态");
	ctx.SaveChanges();
	db.QueryPosts().Dump("场景4 结果");
}

// ====================================================================
// 场景5（异常）：必需关系 + DeleteOrphansTiming = Never，Posts.Clear() → InvalidOperationException
//   关闭了孤儿删除，EF 只把外键置 null（概念上），但关系是必需的 → SaveChanges 抛异常
// ====================================================================
{
	var db = Cascade.CreateDatabase(behavior: null, required: true);
	var blogId = db.Seed();

	Cascade.Try(() =>
	{
		using var ctx = db.NewContext();
		ctx.ChangeTracker.DeleteOrphansTiming = CascadeTiming.Never;

		var blog = ctx.Blogs.Include(b => b.Posts).Single(b => b.Id == blogId);
		blog.Posts.Clear();
		ctx.ChangeTracker.DetectChanges();

		ctx.DumpPostStates("场景5 [Posts.Clear 之后] Post 不再被删除，只是 Modified");
		ctx.SaveChanges();
	}, "场景5 必需关系 + DeleteOrphansTiming.Never + Posts.Clear()");
}
