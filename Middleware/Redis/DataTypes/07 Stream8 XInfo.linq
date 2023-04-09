<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

#load ".\07 Stream5 XGroup"

//缺乏可观察性的消息传递系统很难使用。不知道谁在消费消息，哪些消息正在挂起，
//给定流中活动的一组使用者组，使一切都变得不透明。

//显示消费组个数
//还显示流中的第一条和最后一条消息
db.StreamInfo(key).Dump();

//获取的组的信息
db.StreamGroupInfo(key).Dump();

//获取组的消费者信息
db.StreamConsumerInfo(key,group1).Dump();