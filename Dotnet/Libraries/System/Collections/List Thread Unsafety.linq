<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


var list = new List<int>();


Task.Run(TaskAddList);
Task.Run(TaskAddList);
Task.Run(TaskAddList);


void TaskAddList()
{
	try
	{
		//ArgumentException
		//Source array was not long enough. Check the source index, length, and the array's lower bounds. (Parameter 'sourceArray')
		for (var i = 0; i < 1000000; i++)
		{
			list.Add(i);
		}
	}
	catch(Exception ex)
	{
		ex.Dump();
	}
}

Console.ReadLine();