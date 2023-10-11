<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


//使用Task.Run(Action)方法
Task task = Task.Run(() =>
{
	Console.WriteLine("Task.Run");
});

//Task.Factory.StartNew(Action)
task = Task.Factory.StartNew(() =>
{
	Console.WriteLine("Task.Factory.StartNew");
});

//构造函数创建Task
task = new Task(() =>
{
	Console.WriteLine("new Task(Action)");
});

task.Start();
task.Wait();