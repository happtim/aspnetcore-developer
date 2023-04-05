<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//https://github.com/redis/redis-doc/blob/master/docs/data-types/tutorial.md
//https://redis.io/docs/data-types/tutorial/

// key 最好不要很长
// 太短没有意义的可以也不太好
// 使用结构方式建议。如："user:1001" 代表id为1001用户的数据

//自动创建和删除key
	//当列表留空时，Redis 有责任删除键，或
	//创建 如果键不存在并且我们正在尝试添加元素，则为空列表 例如，使用 LPUSH。

	//当我们向聚合数据类型添加元素时，如果目标键不存在，则在添加元素之前创建一个空的聚合数据类型。
	//当我们从聚合数据类型中删除元素时，如果值保持为空，则会自动销毁键。流数据类型是此规则的唯一例外。
	//使用只读命令（如 LLEN）（返回列表的长度）或使用一个空key删除元素，
	//写入命令始终会产生与键包含命令期望查找的类型的空聚合类型相同的结果。

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();

string value = "abcdefg";
var key = "mykey";

//设置key 的string value。
db.StringSet(key, value);

//更改key和查询key

	//判断key是否存在
	db.KeyExists(key).Dump("exists");

	//获取key的类型
	db.KeyType(key).Dump("keytype");

	//删除key
	db.KeyDelete(key);
	
//key过期
	db.StringSet(key, value);

	//设置到期时间
	db.KeyExpire(key,TimeSpan.FromMilliseconds(200));
	
	db.KeyExpireTime(key).Dump("expire time");

	db.StringGet(key).ToString().Dump("before expire");

	Thread.Sleep(500);

	db.StringGet(key).ToString().Dump("after expire");
	
//自动创建和删除key
	//列子1：
	var key2 = "mylist";
	db.KeyDelete(key2);
	db.ListLeftPush(key2, new RedisValue[] { 1, 2, 3});
	
	db.StringSet(key, value);
	//db.ListLeftPush(key, new RedisValue[] { 1, 2, 3});
	

	//例子2：
	db.KeyExists(key2).Dump("key2 exists");
	db.ListLeftPop(key2);
	db.ListLeftPop(key2);
	db.ListLeftPop(key2);
	db.KeyExists(key2).Dump("key2 exists");
	
	//例子3：
	db.KeyDelete(key2).Dump("key2 deleted");
	db.ListLength(key2).Dump("key2 length");
	db.ListLeftPop(key2).IsNull.Dump("key2 IsNull");