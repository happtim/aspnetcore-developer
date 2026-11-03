<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
</Query>

//Dictionary 多线程操作异常情况

var dics = new Dictionary<long,long>();


Task.Run(() => 
{
	try
	{
		while (true)
		{
			//InvalidOperationException
			//Collection was modified; enumeration operation may not execute.
			//foreach( var l in  dics.Select(kp => kp.Value).ToList()){}
			
			//ArgumentException
			//Destination array is not long enough to copy all the items in the collection. Check array index and length.
			var list =dics.ToList().Select(kp => kp.Value);
			Thread.Sleep(5);
		}
	}
	catch(Exception ex)
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

			long l =new Random().NextInt64();
			dics.Add(l,l);
			
			Thread.Sleep(5);
		}
	}
	catch (Exception ex)
	{
		ex.Dump();
	}
});

Console.ReadLine();