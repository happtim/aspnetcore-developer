<Query Kind="Statements" />

//https://learn.microsoft.com/zh-cn/dotnet/api/system.io.streamwriter

//实现一个 TextWriter，使其以一种特定的编码向流中写入字符。
//继承Object -> MarshalByRefObject -> TextWriter -> StreamWriter

Directory.GetCurrentDirectory().Dump();

// Get the directories currently on the C drive.
DirectoryInfo[] cDirs = new DirectoryInfo(@"c:\").GetDirectories();

// Write each directory name to a file.
using (StreamWriter sw = new StreamWriter("./CDriveDirs.txt"))
{
	foreach (DirectoryInfo dir in cDirs)
	{
		sw.WriteLine(dir.Name);
	}
}

// Read and show each line from the file.
string line = "";
using (StreamReader sr = new StreamReader("./CDriveDirs.txt"))
{
	while ((line = sr.ReadLine()) != null)
	{
		Console.WriteLine(line);
	}
}
