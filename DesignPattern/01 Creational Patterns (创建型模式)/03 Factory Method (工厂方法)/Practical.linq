<Query Kind="Statements" />



LoggerFactory factory = new FileFactory(); //可引入配置文件实现    
Logger logger = factory.createLogger();
logger.writeLog();

public class FileFactory : LoggerFactory
{
	public override Logger createLogger()
	{
		return new FileLogger();
	}
}

public class FileLogger : Logger
{
	public override void writeLog()
	{
		Console.WriteLine("文件记录日志");
	}
}

public class DatabaseFactory : LoggerFactory
{

	public override Logger createLogger()
	{
		return new DatabaseLogger();
	}
}

public class DatabaseLogger : Logger
{

	public override void writeLog()
	{
		Console.WriteLine("数据库记录日志");
	}
}


public abstract class Logger
{
	public abstract void writeLog();
}

public abstract class LoggerFactory
{
	public abstract Logger createLogger();
}