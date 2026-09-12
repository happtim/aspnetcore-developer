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

// 级联删除（1/4）：问题由来、EF 默认约定、以及"EF 端"与"数据库端"两个层面
//
// 问题：删除父实体（Blog）后，子实体（Post）的外键 BlogId 指向一个不存在的行，
//       这是无效状态，数据库会报外键约束错误。只有两种出路：
//         a. 连子实体一起删（级联删除）
//         b. 把子实体的外键置为 null（要求外键可空）
//
// 特征 1 —— EF 的默认约定（不写 OnDelete 时）：
//   必需关系（外键不可空 / IsRequired(true)）  →  DeleteBehavior.Cascade
//   可选关系（外键可空   / IsRequired(false)） →  DeleteBehavior.ClientSetNull
//
// 特征 2 —— 级联删除发生在两个层面，缺一不可：
//   EF 端     ：只作用于"当前 context 已跟踪"的子实体。删父实体时 EF 在内存里把子实体标记为 Deleted
//               （或把外键置 null），SaveChanges 时为每个子实体单独发 DELETE / UPDATE。
//   数据库端  ：只作用于"没有加载进 context"的子实体。靠建表时外键约束上的 ON DELETE CASCADE / SET NULL，
//               EF 只发一条 DELETE FROM Blogs，剩下的由数据库完成。
//   名字带 Client 前缀的行为（ClientCascade / ClientSetNull / ClientNoAction）只有 EF 端，
//   数据库端不生成 ON DELETE 子句 —— 子实体没加载时就会报外键错误（见 3_Exceptions）。

#load ".\CascadeContext"

// ====================================================================
// 场景1：必需关系 + 不配置 OnDelete → 默认 Cascade，建表 SQL 带 ON DELETE CASCADE
// ====================================================================
var requiredDb = Cascade.CreateDatabase(behavior: null, required: true);
new
{
	关系 = "必需  IsRequired(true)",
	模型中的DeleteBehavior = requiredDb.FkDeleteBehavior,   // Cascade
	建表外键约束 = requiredDb.ForeignKeyDdl,                 // ... ON DELETE CASCADE
}.Dump("场景1 必需关系的默认约定");

// ====================================================================
// 场景2：可选关系 + 不配置 OnDelete → 默认 ClientSetNull，建表 SQL 没有 ON DELETE 子句
// ====================================================================
var optionalDb = Cascade.CreateDatabase(behavior: null, required: false);
new
{
	关系 = "可选  IsRequired(false)",
	模型中的DeleteBehavior = optionalDb.FkDeleteBehavior,   // ClientSetNull
	建表外键约束 = optionalDb.ForeignKeyDdl,                 // 没有 ON DELETE → 数据库端 NO ACTION
}.Dump("场景2 可选关系的默认约定");

// ====================================================================
// 场景3：EF 端级联 —— 子实体已加载（Include），由 EF 在内存里级联
//   Remove(blog) 之后、SaveChanges 之前，3 个 Post 就已经是 Deleted 状态（默认级联时机 Immediate）
//   SaveChanges 日志：3 条 DELETE FROM "Posts" + 1 条 DELETE FROM "Blogs"
// ====================================================================
{
	var blogId = requiredDb.Seed();
	using var ctx = requiredDb.NewContext();

	var blog = ctx.Blogs.Include(b => b.Posts).Single(b => b.Id == blogId);
	ctx.Remove(blog);

	ctx.DumpPostStates("场景3 [Remove 之后、SaveChanges 之前] Post 已被 EF 标记为 Deleted");

	"下面的日志应有 4 条 DELETE：3 条 Posts + 1 条 Blogs".Dump("场景3 EF 端级联");
	ctx.SaveChanges();

	new { 数据库中剩余Post数 = requiredDb.QueryPosts().Count }.Dump("场景3 结果");
}

// ====================================================================
// 场景4：数据库端级联 —— 子实体没有加载，EF 只删 Blog，Post 由数据库的 ON DELETE CASCADE 删除
//   SaveChanges 日志：只有 1 条 DELETE FROM "Blogs"，但查询 Posts 表也已经空了
// ====================================================================
{
	var blogId = requiredDb.Seed();
	using var ctx = requiredDb.NewContext();

	var blog = ctx.Blogs.Find(blogId);   // 不 Include，Post 不在 context 里
	ctx.Remove(blog);

	ctx.DumpPostStates("场景4 [Remove 之后] context 里没有任何 Post，EF 端无事可做");

	"下面的日志只有 1 条 DELETE FROM Blogs，Post 由数据库级联删除".Dump("场景4 数据库端级联");
	ctx.SaveChanges();

	new { 数据库中剩余Post数 = requiredDb.QueryPosts().Count }.Dump("场景4 结果");
}

// ====================================================================
// 小结
//   * 同一个 DeleteBehavior.Cascade，子实体加载了就由 EF 逐条删，没加载就由数据库删，结果一样。
//   * 想让两个层面都成立，需要数据库是由 EF（Migrations / EnsureCreated）建的，
//     否则数据库端的 ON DELETE 子句可能不存在，此时只能靠 EF 端，等价于 ClientCascade。
//   * 7 种 DeleteBehavior 的完整对照见 CascadeDelete2_BehaviorMatrix。
// ====================================================================
