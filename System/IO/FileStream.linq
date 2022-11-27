<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//https://learn.microsoft.com/zh-cn/dotnet/standard/io/

//抽象基类 Stream 支持读取和写入字节。 所有表示流的类都继承自 Stream 类。

//流涉及三个基本操作：
//读取 - 将数据从流传输到数据结构（如字节数组）中。
//写入 - 将数据从数据源传输到流。
//查找 - 对流中的当前位置进行查询和修改。

//下面是一些常用的流类：
//FileStream - 用于对文件进行读取和写入操作。
//MemoryStream - 用于作为后备存储对内存进行读取和写入操作。
//NetworkStream - 用于通过网络套接字进行读取和写入。

string path = @".\MyTest.txt";

Directory.GetCurrentDirectory().Dump();
// Delete the file if it exists.
if (File.Exists(path))
{
	File.Delete(path);
}

//File静态类 内部使用 new FileStream创建流。默认参数 FileMode.Create, FileAccess.ReadWrite, FileShare.None, 
//FileAccess:
//Read：对文件的读访问。 可从文件中读取数据。 与 Write 组合以进行读写访问。
//ReadWrite：对文件的读写访问权限。 可从文件读取数据和将数据写入文件。
//Write：件的写访问。 可将数据写入文件。 与 Read 组合以进行读写访问。

//using (FileStream fs = File.Create(path))
using (FileStream fs = new FileStream(path,FileMode.Create,FileAccess.Write))
{
	AddText(fs, "This is some text");
	AddText(fs, "This is some more text,");
	AddText(fs, "\r\nand this is on a new line");
	AddText(fs, "\r\n\r\nThe following is a subset of characters:\r\n");

	for (int i = 1; i < 120; i++)
	{
		AddText(fs, Convert.ToChar(i).ToString());
	}
}

static void AddText(FileStream fs, string value)
{
	byte[] info = new UTF8Encoding(true).GetBytes(value);
	fs.Write(info, 0, info.Length);
}