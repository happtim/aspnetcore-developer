<Query Kind="Statements" />

// 创建一个内存流
using (MemoryStream memoryStream = new MemoryStream())
{
	// 使用StreamWriter向内存流中写入文本数据
	using (StreamWriter streamWriter = new StreamWriter(memoryStream))
	{
		// 写入文本数据
		streamWriter.WriteLine("Hello, World!");
		streamWriter.WriteLine("This is a test.");
		streamWriter.WriteLine("Stream writing example.");
	}

	// 将内存流的内容转换为字节数组并输出
	byte[] buffer = memoryStream.ToArray();
	Console.WriteLine(System.Text.Encoding.UTF8.GetString(buffer));
}