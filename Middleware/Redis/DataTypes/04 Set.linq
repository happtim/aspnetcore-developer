<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

//https://redis.io/docs/data-types/sets/

//Redis 集是唯一字符串（成员）的无序集合。 您可以使用 Redis 集有效地：

//跟踪唯一项目（例如，跟踪访问给定博客文章的所有唯一 IP 地址）。
//表示关系（例如，具有给定角色的所有用户的集合）。
//执行常见的集合操作，例如交集、并集和差分。


//集合适用于表达对象之间的关系。 
	//例如，我们可以轻松地使用集合来实现标签。
	//对此问题进行建模的一种简单方法是为每个对象设置一个集合 想要标记。该集包含与对象关联的标记的 ID。
	//一个例子是标记新闻文章。 如果文章 ID 1000 标记了标记 1、2、5 和 77，则一组 可以将这些标签 ID 与新闻项相关联

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();

//存储用户 123 和 456 的收藏书籍 ID 集：

var key1 = "user:123:favorites";
var key2 = "user:456:favorites";
db.SetAdd(key1,347);
db.SetAdd(key1,561);
db.SetAdd(key1,742);

db.SetAdd(key2,561);

//查看所有集合
db.SetMembers(key1).Select(v => (int)v).Dump();

//随机获取一个元素
((int)db.SetRandomMember(key1)).Dump("random member");

//用户 123 喜欢多少本书？
db.SetLength(key1).Dump("how many book user 123 favorited");

//检查用户 123 是否喜欢书籍 742 和 299
db.SetContains(key1,742).Dump("user 123 whether like 742");
db.SetContains(key1,299).Dump("user 123 whether like 299");

//用户 123 和 456 有什么共同喜欢的书吗？
db.SetCombine( SetOperation.Intersect , key1,key2).Select(v =>(int)v ).Dump("user 123 and 456 have any favorite books in common");

