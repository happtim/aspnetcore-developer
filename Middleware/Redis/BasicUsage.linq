<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

//https://github.com/redis/redis-doc/blob/master/docs/data-types/tutorial.md

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();

string value = "abcdefg";
var key = "mykey";

//设置key 的string value。

//StackExchange.Redis 用 RedisValue 类型表示值。 
//与 RedisKey 一样，存在隐式转换，这意味着大多数时候你从来没有看到这种类型，例如：
db.StringSet(key, value);

db.StringGet(key).ToString().Dump();


//除了文本和二进制内容，值还可能需要表示类型化的原始数据 - 最常见的（在.NET术语中）Int32，Int64，Double或Boolean。 
//因此，RedisValue提供了比 RedisKey 更多的转换支持：

//public static implicit operator RedisValue(int value)
//public static explicit operator int(RedisValue value)
db.StringSet("mykey", 123); // this is still a RedisKey and RedisValue
int i = (int)db.StringGet("mykey");


//另外注意，当做数字处理时，redis将不存在的键视为零.
db.KeyDelete("abc");
((int)db.StringGet("abc")).Dump("StringGet"); // this is ZERO

//如果您需要检测空状态，那么你就可以这样检查：
db.KeyDelete("abc");
db.StringGet("abc").IsNull.Dump("abc is null");
