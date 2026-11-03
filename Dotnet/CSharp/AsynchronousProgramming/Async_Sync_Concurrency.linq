<Query Kind="Statements">
  <Namespace>System.Windows</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


var stack = new System.Windows.Controls.StackPanel() {MaxWidth = 300};
var margin = new Thickness(2);

var button1 = new System.Windows.Controls.Button() { Content = "Sync",Margin = margin };
var button2 = new System.Windows.Controls.Button() { Content = "Async",Margin = margin };
var button3 = new System.Windows.Controls.Button() { Content = "Concurrency", Margin = margin };
var button4 = new System.Windows.Controls.Button() { Content = "ContinueWith", Margin = margin };
var button5 = new System.Windows.Controls.Button() { Content = "ConfigureAwait", Margin = margin };
var button6 = new System.Windows.Controls.Button() { Content = "Wait", Margin = margin };
var button7 = new System.Windows.Controls.Button() { Content = "Deadlock", Margin = margin };
var text = new System.Windows.Controls.TextBox() { Height = 200};

stack.Children.Add(button1);
stack.Children.Add(button2);
stack.Children.Add(button3);
stack.Children.Add(button4);
stack.Children.Add(button5);
stack.Children.Add(button6);
stack.Children.Add(button7);
stack.Children.Add(text);


var panel = PanelManager.DisplayWpfElement(stack, "stackpanel");


button1.Click += (sender, args) =>
{
	foreach (var element in Enumerable.Range(0,5))
	{
		var result =  Fun();
		text.AppendText( result + "\r\n");
	} 
};

button2.Click += async (sender, args) =>
{
	foreach (var element in Enumerable.Range(0, 5))
	{
		var result = await Task.Run(() =>  Fun());
		
		//var result = await FunAsync();
		
		text.AppendText( result + "\r\n");
	}
};

button3.Click += async(sender, args) =>
{
	var tasks =  Enumerable.Range(0, 5).Select(t => Task.Run(() => Fun())).ToList();
	var results =  await Task.WhenAll(tasks);
	
	foreach (var result in results)
	{
		text.AppendText( result + "\r\n");
	}
};

button4.Click += async (sender, args) =>
{
	foreach (var element in Enumerable.Range(0, 5))
	{
		await Task.Run(() => Fun()).ContinueWith(t =>
		{
			//如果直接返回出现异常， 需要切换成UI线程去执行。
			//text.AppendText( t.Result + "\r\n");
			panel.GetControl().Invoke(() => {text.AppendText( t.Result + "\r\n");});
		});
		
	}
};

//ConfigureAwait 是一个用于控制 await 操作的关键字，它用于指定是否在 await 操作之后继续运行在原始线程中或者切换到新的线程中。
//ConfigureAwait(true)：如果前一个 await 操作是在 UI 线程上执行的，那么在 await 操作完成后，
//代码将切回到 UI 线程中执行。这对于在处理 GUI 应用程序时很有用。

//ConfigureAwait(false)：无论前一个 await 操作在哪个线程上执行，await 操作完成后不切回到原始上下文，
//而是继续在其他线程上执行。这对于避免上下文切换和提高性能很有用。

//使用 ConfigureAwait(false) 可以帮助避免线程上下文切换的开销，并且在高并发的情况下提高性能。
//但是如果你需要在 await 操作之后访问 UI 控件，或者确保在 UI 线程上执行某些代码，那么就需要使用 ConfigureAwait(true)

button5.Click += async (sender, args) =>
{
	text.AppendText("UI ThreadId: " + Thread.CurrentThread.ManagedThreadId + "\r\n");
	
	foreach (var element in Enumerable.Range(0, 5))
	{
		var result = await Task.Run(() => Fun()).ConfigureAwait(true);
		int threadId = Thread.CurrentThread.ManagedThreadId;
		panel.GetControl().Invoke(() => {text.AppendText(result + " ThreadId: " + threadId + "\r\n");});
	}
};

button6.Click += (sender, args) =>
{
	foreach (var element in Enumerable.Range(0, 5))
	{
		var task = Task.Run(() => Fun());
		
		//调用线程阻塞在Wait处，当任务完成，取消或者异常
		//task.Wait();
		text.AppendText(task.Result + "\r\n");
	}
};

button7.Click += (sender, args) =>
{
	foreach (var element in Enumerable.Range(0, 5))
	{
		//而 async 函数并不会启动新线程，它仅仅是一种编写和管理异步代码的语法糖，并使用现有的线程池线程或 UI 线程来执行异步操作。
		var task = FunAsync();
		
		//Task.Run 会在 ThreadPool 中启动一个新的线程，将指定的方法作为一个新的工作项进行执行。这会导致异步操作在一个不同的线程上执行。
		//var task = Task.Run( async () =>
		//{
		//	await Task.Delay(1000);
		//	return "死锁了";
		//});
		
		//task.Wait();
		text.AppendText(task.Result + "\r\n");
	}
};

async Task<string> DeadlockAsync()
{
	await Task.Delay(1000);
	return "死锁了";
}

async Task<string> FunAsync()
{
	await Task.Delay(1000);
	return "call function";
}

string Fun()
{
	Thread.Sleep(1000);
	return "call function";
}