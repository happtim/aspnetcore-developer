<Query Kind="Statements" />


RegisterCallback("Way1",DisplayMessage); // 注册回调方法
RegisterCallback("Way2",delegate (string message) { Console.WriteLine("Message received: " + message); }); // 注册回调方法
RegisterCallback("Way3",message => Console.WriteLine("Message received: " + message)); // 注册回调方法

//RegisterCallback 该方法不用关注其实现，他的作用就是简单讲参传递给其他函数。
static void RegisterCallback(string way ,CallbackDelegate callback)
{
	// 模拟一些操作
	//Console.WriteLine("RegisterCallback method started.");
	callback(way +" callback called"); // 调用回调方法
	//Console.WriteLine("RegisterCallback method completed.");
}

static void DisplayMessage(string message)
{
	Console.WriteLine("Message received: " + message);
}

delegate void CallbackDelegate(string message);