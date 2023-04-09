<Query Kind="Statements" />

#load ".\07 Stream5 XGroup"


//获取组中的pending信息。
var pending = db.StreamPending(key,group1);
//pending.Dump();
pending.PendingMessageCount.Dump("total number of pending messages ");
((string)pending.Consumers[0].Name).Dump("pending user");
((string)pending.HighestPendingMessageId).Dump("higher message ID");
((string)pending.LowestPendingMessageId).Dump("lower message ID");

//获取挂起消息的更多信息
Thread.Sleep(20); //增加一些延时时间。
var pendingMessage =db.StreamPendingMessages(key,group1,2,"Bob",pending.LowestPendingMessageId);

//pendingMessage.Dump();
pendingMessage[0].DeliveryCount.Dump("DeliveryCount");
pendingMessage[0].IdleTimeInMilliseconds.Dump("IdleTimeInMilliseconds");

//xclaim 可以将挂起的消息 变更所有权。
//minIdleTime 20 秒，和 指定id 全部满足的消息转给 Alice处理。
db.StreamClaim(key,group1,"Alice",20,pendingMessage.Select(id =>id.MessageId ).ToArray());
pendingMessage =db.StreamPendingMessages(key,group1,2,"Alice",pending.LowestPendingMessageId);
pendingMessage.Dump();

