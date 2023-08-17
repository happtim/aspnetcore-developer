<Query Kind="Statements" />

// 创建一个内存流，并写入一些文本数据
using (MemoryStream memoryStream = new MemoryStream())
{
	byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Hello, World!");

	memoryStream.Write(buffer, 0, buffer.Length);
	memoryStream.Position = 0; // 重置流位置为起始位置

	// 使用StreamReader读取流中的文本数据
	using (StreamReader streamReader = new StreamReader(memoryStream))
	{
		string line;
		while ((line = streamReader.ReadLine()) != null)
		{
			Console.WriteLine(line);
		}
	}
}