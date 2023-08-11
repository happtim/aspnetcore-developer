<Query Kind="Statements" />



//1. 使用匹配签名声明委托类型并声明方法：
MyDelegate myDelegate = new MyDelegate(MyMethod);
myDelegate("hello world");

//2. 将方法分配给委托类型：
myDelegate = MyMethod;
myDelegate("hello world 2");

myDelegate = SampleClass.StaticMethod;
myDelegate("hello world 2-1");

myDelegate = new SampleClass().InstanceMethod;
myDelegate("hello world 2-2");

//3. 声明匿名方法：
myDelegate = delegate (string message)
{
	Console.WriteLine(message);
};
myDelegate("hello world 3");


//4. lambda
myDelegate = name => Console.WriteLine(name);
myDelegate("hello world 4");


static void MyMethod(string message)
{
	Console.WriteLine(message);
}

delegate void MyDelegate(string message);

class SampleClass
{
	public void InstanceMethod(string message)
	{
		Console.WriteLine(message);
	}

	static public void StaticMethod(string message)
	{
		Console.WriteLine(message);
	}
}



