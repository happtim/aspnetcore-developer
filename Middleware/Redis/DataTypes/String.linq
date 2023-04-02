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
db.StringSet(key, value);

//设置新值，返回旧的值
db.StringGetSet(key, "higklmn").Dump("StringGetSet");

//获取值
db.StringGet(key).Dump("StringGet");
