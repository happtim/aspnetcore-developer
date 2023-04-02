<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

//https://github.com/redis/redis-doc/blob/master/docs/data-types/tutorial.md

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();

string value = "abcdefg";
var key = "mykey";

// key 
// key 最好不要很长
// 太短没有意义的可以也不太好
// 使用结构方式建议。如："user:1001" 代表id为1001用户的数据

//设置key 的string value。
db.StringSet(key, value);

//判断key是否存在
db.KeyExists(key).Dump();

//获取key的类型
db.KeyType(key).Dump();

//设置新值，返回旧的值
db.StringGetSet(key, "higklmn").Dump("StringGetSet");

//获取值
db.StringGet("mykey").Dump("StringGet");

//删除key
db.KeyDelete(key);


//删除key
db.KeyDelete(key);