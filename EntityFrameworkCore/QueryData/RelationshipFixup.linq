<Query Kind="Statements">
  <NuGetReference Version="6.0.25">Microsoft.EntityFrameworkCore.Sqlite</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>Microsoft.Data.Sqlite</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Infrastructure</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Storage</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

// 关联关系修正 (Relationship Fix-up)
// EF Core 的变更跟踪器在实体进入追踪状态时，自动检查并修正所有导航属性与外键之间的一致性。
// 无需手动同步，无论以何种顺序加载关联实体，EF Core 都会确保两端保持一致。
//
// 三种触发场景：
// 1. 先加载引用端(Blog)，再加载依赖端(Post) → Post.Blog 与 Blog.Posts 自动建立关联
// 2. 先加载依赖端(Post)，再加载引用端(Blog) → 同上，顺序无关
// 3. 修改导航属性(post.Blog = blog2)          → post.BlogId 及双方集合自动同步更新

#load ".\Context"

var connection = CreateDatabaseAndGetConnection();
var options = new DbContextOptionsBuilder<MyContext>().UseSqlite(connection).Options;

// 准备测试数据
int blog1Id, blog2Id, postId;
using (var ctx = new MyContext(options))
{
	var blog1 = new Blog { Url = "https://blog1.com" };
	var blog2 = new Blog { Url = "https://blog2.com" };
	ctx.Blogs.AddRange(blog1, blog2);
	ctx.SaveChanges();
	blog1Id = blog1.Id;
	blog2Id = blog2.Id;

	var post = new Post { Title = "Fix-up Demo Post", BlogId = blog1Id };
	ctx.Posts.Add(post);
	ctx.SaveChanges();
	postId = post.Id;
}

// ====================================================================
// 场景1：先加载引用端 (Blog)，再加载依赖端 (Post)
// Fix-up：Post 被追踪时，EF Core 发现其 BlogId 匹配已追踪的 Blog.Id，
//         自动将 post.Blog 指向该 Blog，并将 post 加入 blog.Posts 集合
// ====================================================================
using (var ctx = new MyContext(options))
{
	var blog = ctx.Blogs.Find(blog1Id);

	$"[步骤1] 仅加载 Blog，blog.Posts.Count = {blog.Posts.Count}（尚未关联）"
		.Dump("场景1");

	// 单独加载 Post，不使用 Include
	var post = ctx.Posts.Find(postId);

	// Fix-up 触发：两个实体都在追踪器中，EF Core 立即修正关联关系
	new
	{
		post_Blog_已自动设置 = post.Blog != null,
		post_Blog_Url = post.Blog?.Url,       // 自动修正：指向已追踪的 blog
		blog_Posts_Count = blog.Posts.Count,  // 自动修正：post 被加入集合
		两端一致 = ReferenceEquals(post.Blog, blog) && blog.Posts.Contains(post),
	}.Dump("场景1 [步骤2] 加载 Post 后（Fix-up 已触发）");
}

// ====================================================================
// 场景2：先加载依赖端 (Post)，再加载引用端 (Blog)
// Fix-up：Blog 被追踪时，EF Core 发现已追踪的 Post.BlogId 匹配 Blog.Id，
//         顺序与场景1相反，结果完全一致
// ====================================================================
using (var ctx = new MyContext(options))
{
	var post = ctx.Posts.Find(postId);

	$"[步骤1] 仅加载 Post，post.Blog is null: {post.Blog == null}（尚未关联）"
		.Dump("场景2");

	// 单独加载 Blog，不使用 Include
	var blog = ctx.Blogs.Find(blog1Id);

	// Fix-up 触发：加载顺序不同，结果相同
	new
	{
		post_Blog_已自动设置 = post.Blog != null,
		post_Blog_Url = post.Blog?.Url,       // 自动修正：指向已追踪的 blog
		blog_Posts_Count = blog.Posts.Count,  // 自动修正：post 被加入集合
		两端一致 = ReferenceEquals(post.Blog, blog) && blog.Posts.Contains(post),
	}.Dump("场景2 [步骤2] 加载 Blog 后（Fix-up 已触发）");
}

// ====================================================================
// 场景3：修改导航属性 → 外键与集合导航属性同步修正
// Fix-up：将 post.Blog 从 blog1 改为 blog2 时，
//         post.BlogId 自动更新，blog1.Posts 减少，blog2.Posts 增加
// ====================================================================
using (var ctx = new MyContext(options))
{
	var blog1 = ctx.Blogs.Find(blog1Id);
	var blog2 = ctx.Blogs.Find(blog2Id);
	var post = ctx.Posts.Find(postId); // Fix-up 已将 post.Blog = blog1

	new
	{
		post_BlogId = post.BlogId,
		post_Blog_Url = post.Blog?.Url,
		blog1_Posts_Count = blog1.Posts.Count,
		blog2_Posts_Count = blog2.Posts.Count,
	}.Dump("场景3 [修改前]");

	post.Blog = blog2; // 修改导航属性
	ctx.ChangeTracker.DetectChanges(); // 显式触发 Fix-up（使外键与集合同步）

	new
	{
		post_BlogId = post.BlogId,              // 自动从 blog1Id 变为 blog2Id
		post_Blog_Url = post.Blog?.Url,         // 自动变为 "https://blog2.com"
		blog1_Posts_Count = blog1.Posts.Count,  // 自动从 1 变为 0
		blog2_Posts_Count = blog2.Posts.Count,  // 自动从 0 变为 1
		外键与导航一致 = post.BlogId == blog2Id && ReferenceEquals(post.Blog, blog2),
	}.Dump("场景3 [修改后]（Fix-up 已触发）");
}
