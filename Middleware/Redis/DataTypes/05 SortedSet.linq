<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

//https://redis.io/docs/data-types/sorted-sets/

//Redis 排序集是按关联分数排序的唯一字符串（成员）的集合。 value 不会重复，重复设置会覆盖
//当多个字符串具有相同的分数时，字符串将按字母顺序排序。

//基本命令
	//ZADD 将新成员和关联的分数添加到排序集。如果成员已存在，则会更新分数。
	//ZRANGE 返回在给定范围内排序的排序集的成员。
	//ZRANK 返回所提供成员的排名，排序是按升序排列的。
	//ZREVRANK 返回所提供成员的排名，排序集按降序排列。

//排序集的一些用例包括：
	//排行榜。例如，您可以使用排序集轻松维护大型在线游戏中最高分的有序列表。
	//速率限制器。特别是，您可以使用排序集来构建滑动窗口速率限制器，以防止过多的 API 请求。


ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();

//随着玩家分数的变化更新实时排行榜：
var key = "leaderboard:455";

db.SortedSetAdd(key, "user:1", 100);
db.SortedSetAdd(key, "user:2",75);
db.SortedSetAdd(key, "user:3",101);
db.SortedSetAdd(key, "user:4",15);
db.SortedSetAdd(key, "user:5",100);
//在最终的 SortedSetAdd 调用中更新了 的分数。user:2
db.SortedSetAdd(key, "user:2",275);

//获取分数在100，200之前的用户
db.SortedSetRangeByScore(key,100,200).Select(v => v.ToString()).Dump("scores between 100 and 200");

//获取前 3 名玩家的分数：
db.SortedSetRangeByRank(key,0,2,Order.Descending).Select(v => (string)v).Dump("the top 3 players' scores");

//用户 2 的等级是多少？
db.SortedSetRank(key,"user:2").Dump("rank of user 2");

//用户 2 的等级倒数是多少？
db.SortedSetRank(key,"user:2",Order.Descending).Dump("revrank of user 2");
