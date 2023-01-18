<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//Dictionary 多线程操作异常情况

var dics = new Dictionary<int, int>();


Task.Run(() =>
{
	try
	{
		while (true)
		{
			
			int i = new Random().Next(100);
			if(dics.ContainsKey(i))
			{
				dics.Remove(i);
			}

			Thread.Sleep(1);
		}
	}
	catch (Exception ex)
	{
		ex.Dump();
	}
});


Task.Run(() =>
{
	try
	{
		while (true)
		{
			//IndexOutOfRangeException 
			//Index was outside the bounds of the array.
			int i = new Random().Next(100);
			if (!dics.ContainsKey(i))
			{
				dics.Add(i,i);
			}

			Thread.Sleep(1);
		}
	}
	catch (Exception ex)
	{
		ex.Dump();
	}
});

Console.ReadLine();