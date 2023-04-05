<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

//https://redis.io/docs/data-types/lists/

//Redis 列表是字符串值的链接列表。 Redis 列表经常用于：
//
//实现堆栈和队列。
//为后台工作程序系统构建队列管理。

//基本命令
	//LPUSH 在列表的头部添加一个新元素; RPUSH添加到尾部。
	//LPOP 从列表的头部删除并返回一个元素; RPOP 执行相同的操作，但从列表的尾部。
	//LLEN 返回列表的长度。
	//LMOVE 以原子方式将元素从一个列表移动到另一个列表。
	//LTRIM 将列表缩减到指定的元素范围。


//列表对于许多任务很有用，这是两个非常有代表性的用例 如下：
	//
	//记住用户发布到社交网络中的最新更新。
	//流行的Twitter社交网络将用户发布的最新推文放入Redis列表中。

	//假设您的主页显示最新的 照片发布在照片共享社交网络中，并且您希望加快访问速度。
	//每次用户发布新照片时，我们都会将其 ID 添加到带有 LPUSH 的列表中。
	//当用户访问主页时，我们使用以获取最新发布的10个项目。LRANGE 0 9

	//进程之间的通信，使用使用者 - 生产者模式，其中生产者将项目推送到列表中，
	//使用者（通常是工作人员）使用这些项目并执行操作。Redis 具有特殊的列表命令，使此用例更加可靠和高效。
	//例如，流行的Ruby库resque和sidekiq都使用Redis list，以便 实现后台作业。


//将列表视为队列（先进先出）

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();

var key1 = "work:queue:ids";
var key2 = "board:in-progress:ids";

db.ListLeftPush(key1,101);
db.ListLeftPush(key1,237);

((int)db.ListRightPop(key1)).Dump("出队");
((int)db.ListRightPop(key1)).Dump("出队");


//将列表视为堆栈（先进后出）

db.ListLeftPush(key1, 101);
db.ListLeftPush(key1, 237);

((int)db.ListLeftPop(key1)).Dump("出栈");
((int)db.ListLeftPop(key1)).Dump("出栈");

//检查列表的长度：
db.ListLength(key1).Dump("列表长度");

//以原子方式从一个列表中弹出一个元素并推送到另一个列表：
db.ListLeftPush(key1, 101);
db.ListLeftPush(key1, 237);

//当前windows不支持
//db.ListMove(key1,key2, ListSide.Left, ListSide.Left);
db.ListRange(key1).Dump();
db.ListRange(key2).Dump();


var key3 = "notifications:user:1";
//要创建一个永远不会超过 100 个元素的上限列表，您可以在每次调用 LPUSH 后调用 LTRIM：

db.ListLeftPush(key3,"You've got mail!");
db.ListTrim(key3,0,99);
db.ListLeftPush(key3,"Your package will be delivered at 12:01 today.");
db.ListTrim(key3,0,99);


