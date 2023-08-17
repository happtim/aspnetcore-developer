<Query Kind="Statements" />

string dir =  Directory.GetCurrentDirectory().Dump();

using (StreamWriter w = File.CreateText("./log.txt"))
//using (StreamWriter w = File.AppendText("./log.txt"))
{
	Log("Test1", w);
	Log("Test2", w);
}

static void Log(string logMessage, TextWriter w)
{
	w.Write("\r\nLog Entry : ");
	w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
	w.WriteLine("  :");
	w.WriteLine($"  :{logMessage}");
	w.WriteLine("-------------------------------");
}

