<Query Kind="Statements">
  <Namespace>System.Runtime.Serialization.Formatters.Binary</Namespace>
</Query>

//新增Invoker
ConfigSettingWindow csw = new ConfigSettingWindow();
Command command = null;
//新建Receiver 接收发送来的消息
ConfigOperator configOperator = new ConfigOperator();

//新增命令
command = new InsertCommand("增加", "");
//设置Receiver
command.SetConfigOperator(configOperator);
//Invoker设置command
csw.SetCommnad(command);
//调用
csw.Call("网站首页");

command = new ModifyCommand("修改", "");
command.SetConfigOperator(configOperator);
csw.SetCommnad(command);
csw.Call("端口号");

command = new DeleteCommand("删除", "");
command.SetConfigOperator(configOperator);
csw.SetCommnad(command);
csw.Call("端口号");

Console.WriteLine("----------------------");
Console.WriteLine("保存配置");
csw.Save();

Console.WriteLine("----------------------");
Console.WriteLine("恢复配置");
csw.Recover();

public class ConfigSettingWindow
{

	private List<Command> commands = new List<Command>();
	private Command command;

	public void SetCommnad(Command command)
	{
		this.command = command;
	}

	public void Call(string args)
	{
		command.execute(args);
		commands.Add(command);
	}

	public void Save()
	{
		FileStream fs = new FileStream("Commands.data", FileMode.Create);
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(fs, commands);
		fs.Close();
	}

	public void Recover()
	{
		FileStream fs = new FileStream("Commands.data", FileMode.Open);
		BinaryFormatter bf = new BinaryFormatter();
		commands = (List<Command>)bf.Deserialize(fs);
		fs.Close();

		foreach (var command in commands)
		{
			command.execute();
		}
	}

}

[Serializable]
public class ModifyCommand : Command
{

	public ModifyCommand(string name, string args) : base(name, args) { }

	public override void execute()
	{
		this.configOperator.modify(this.args);
	}

	public override void execute(string args)
	{
		this.args = args;
		this.configOperator.modify(this.args);
	}
}

[Serializable]
public class InsertCommand : Command
{

	public InsertCommand(string name, string args) : base(name, args) { }

	public override void execute()
	{
		this.configOperator.insert(this.args);
	}

	public override void execute(string args)
	{
		this.args = args;
		this.configOperator.insert(this.args);
	}


}

[Serializable]
public class DeleteCommand : Command
{

	public DeleteCommand(string name, string args) : base(name, args) { }

	public override void execute()
	{
		this.configOperator.delete(this.args);
	}

	public override void execute(string args)
	{
		this.args = args;
		this.configOperator.delete(this.args);
	}
}

[Serializable]
public class ConfigOperator
{
	public void insert(String args)
	{
		Console.WriteLine("增加新节点：" + args);
	}

	public void modify(String args)
	{
		Console.WriteLine("修改节点：" + args);
	}

	public void delete(String args)
	{
		Console.WriteLine("删除节点：" + args);
	}
}

[Serializable]
public abstract class Command
{
	protected string name;
	protected string args;
	protected ConfigOperator configOperator;

	public Command(string name, string args)
	{
		this.name = name;
		this.args = args;
	}

	public void SetConfigOperator(ConfigOperator configOperator)
	{
		this.configOperator = configOperator;
	}

	public abstract void execute();
	public abstract void execute(string args);
}