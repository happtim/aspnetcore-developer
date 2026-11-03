<Query Kind="Statements">
  <Namespace>System.Windows</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//https://learn.microsoft.com/zh-cn/dotnet/csharp/asynchronous-programming/

// =====================================================================================
// 同步、异步、并发与死锁：在 WPF 界面上从上到下依次点击 8 个按钮，每一步都在上一步的基础上往前推进一点。
//
//   1. Sync            同步调用耗时方法            → UI 冻结 5 秒                    （问题）
//   2. Async           await Task.Run              → UI 不卡，但 5 个调用依次执行 5 秒 （解决卡顿）
//   3. Concurrency     Task.WhenAll                → UI 不卡，5 个调用同时执行 1 秒    （提速）
//   4. ContinueWith    await 出现之前的回调写法    → 要自己切回 UI 线程               （理解 await 替我们做了什么）
//   5. ConfigureAwait  控制 await 之后回不回 UI 线程 → true 回，false 不回             （await 背后的机制）
//   6. Wait            在 UI 线程上 .Result 等线程池任务 → UI 冻结 5 秒，但不死锁       （同步等待异步：sync over async）
//   7. Deadlock        在 UI 线程上 .Result 等 async 方法 → UI 永久卡死                （第 5、6 步合起来的后果）
//   8. Deadlock + ConfigureAwait(false) → 不再死锁，但 UI 仍冻结 5 秒                 （第 5 步的 false 能解开死锁，但治标不治本）
//
// 结论：UI 代码里 async/await 一路到底，不要用 .Result / .Wait() 同步等待；类库代码里的 await 写 ConfigureAwait(false)。
//
// 观察方法：点击按钮后试着拖动 LINQPad 窗口或点击其他按钮。窗口没有反应，说明 UI 线程被占住了。
// =====================================================================================


var stack = new System.Windows.Controls.StackPanel() {MaxWidth = 300};
var margin = new Thickness(2);

var button1 = new System.Windows.Controls.Button() { Content = "1. Sync",Margin = margin };
var button2 = new System.Windows.Controls.Button() { Content = "2. Async",Margin = margin };
var button3 = new System.Windows.Controls.Button() { Content = "3. Concurrency", Margin = margin };
var button4 = new System.Windows.Controls.Button() { Content = "4. ContinueWith", Margin = margin };
var button5 = new System.Windows.Controls.Button() { Content = "5. ConfigureAwait", Margin = margin };
var button6 = new System.Windows.Controls.Button() { Content = "6. Wait", Margin = margin };
var button7 = new System.Windows.Controls.Button() { Content = "7. Deadlock", Margin = margin };
var button8 = new System.Windows.Controls.Button() { Content = "8. Deadlock + ConfigureAwait(false)", Margin = margin };
var text = new System.Windows.Controls.TextBox() { Height = 200};

stack.Children.Add(button1);
stack.Children.Add(button2);
stack.Children.Add(button3);
stack.Children.Add(button4);
stack.Children.Add(button5);
stack.Children.Add(button6);
stack.Children.Add(button7);
stack.Children.Add(button8);
stack.Children.Add(text);


var panel = PanelManager.DisplayWpfElement(stack, "stackpanel");


// -------------------------------------------------------------------------------------
// 第 1 步：Sync（同步）—— 问题从这里开始
//
// 按钮的 Click 事件处理程序运行在 UI 线程上。UI 线程同时负责处理消息循环：重绘界面、响应鼠标和键盘。
// 这里直接调用 Fun()，Fun() 里 Thread.Sleep(1000) 会让当前线程（也就是 UI 线程）什么都不做地等 1 秒。
// 循环 5 次，UI 线程被占住 5 秒，这期间界面无法重绘、无法响应点击，看起来像"卡死"了。
// 另外，5 行文字是 5 秒后一次性显示出来的：每次 AppendText 只是修改了文本，真正重绘要等 UI 线程空出来。
// -------------------------------------------------------------------------------------
button1.Click += (sender, args) =>
{
	foreach (var element in Enumerable.Range(0,5))
	{
		var result =  Fun();
		text.AppendText( result + "\r\n");
	} 
};

// -------------------------------------------------------------------------------------
// 第 2 步：Async（异步）—— 把耗时工作移出 UI 线程
//
// 改动有两处：事件处理程序加上 async；耗时调用改成 await Task.Run(() => Fun())。
//   - Task.Run 把 Fun() 放到线程池线程上执行，UI 线程不再被 Thread.Sleep 占住。
//   - await 遇到还没完成的 Task 时，会把方法剩下的部分注册成"续体"，然后立刻返回，UI 线程回去处理消息循环。
//   - Task 完成后，续体被投递回 UI 线程执行（第 5 步会解释原因），所以 text.AppendText 可以直接访问控件。
// 效果：界面一直能响应，文字每秒出现一行。但 5 次调用仍然是一个接一个执行的，总共还是 5 秒。
//
// 注释掉的 FunAsync() 是另一种写法：Task.Delay 由计时器驱动，等待期间不占用任何线程，
// 是真正的异步 I/O 风格。Task.Run 适合 CPU 密集或只能同步调用的代码；I/O 操作应优先使用原生的 xxxAsync 方法。
// -------------------------------------------------------------------------------------
button2.Click += async (sender, args) =>
{
	foreach (var element in Enumerable.Range(0, 5))
	{
		var result = await Task.Run(() =>  Fun());
		
		//var result = await FunAsync();
		
		text.AppendText( result + "\r\n");
	}
};

// -------------------------------------------------------------------------------------
// 第 3 步：Concurrency（并发）—— 异步不等于并发，要同时跑需要先全部启动再等待
//
// 第 2 步是"启动一个 → 等它完成 → 再启动下一个"。这里先用 Select 一次性启动 5 个 Task，
// 它们在 5 个线程池线程上同时执行；再用 await Task.WhenAll 等待全部完成。
// 注意 .ToList()：它让 5 个 Task 在这一行就全部启动。LINQ 是延迟执行的，不加 ToList 时 Task 会在 WhenAll 枚举序列时才创建。
// 效果：总耗时约 1 秒，5 行结果同时出现。
// -------------------------------------------------------------------------------------
button3.Click += async(sender, args) =>
{
	var tasks =  Enumerable.Range(0, 5).Select(t => Task.Run(() => Fun())).ToList();
	var results =  await Task.WhenAll(tasks);
	
	foreach (var result in results)
	{
		text.AppendText( result + "\r\n");
	}
};

// -------------------------------------------------------------------------------------
// 第 4 步：ContinueWith —— 没有 await 时，"任务完成后接着做什么"要自己写回调
//
// ContinueWith 注册的回调默认在线程池线程上执行（TaskScheduler.Default），而不是 UI 线程。
// WPF 控件只能在创建它的 UI 线程上访问，在线程池线程上直接调用 text.AppendText 会抛 InvalidOperationException（跨线程访问）。
// 这个异常会被记录在 ContinueWith 返回的 Task 里，外层的 await 再把它重新抛出来。
// 所以这里要用 Invoke 手动切回 UI 线程。另一种写法是给 ContinueWith 传 TaskScheduler.FromCurrentSynchronizationContext()。
//
// 对比第 2 步：await 自动完成了"切回 UI 线程"这件事，代码还能像同步代码一样从上往下写。
// 这就是 await 存在的意义。下一步看它是怎么做到的。
// -------------------------------------------------------------------------------------
button4.Click += async (sender, args) =>
{
	foreach (var element in Enumerable.Range(0, 5))
	{
		await Task.Run(() => Fun()).ContinueWith(t =>
		{
			//直接访问控件会抛跨线程异常，需要切换到 UI 线程去执行。
			//text.AppendText( t.Result + "\r\n");
			panel.GetControl().Invoke(() => {text.AppendText( t.Result + "\r\n");});
		});
		
	}
};

// -------------------------------------------------------------------------------------
// 第 5 步：ConfigureAwait —— await 之后为什么能回到 UI 线程
//
// UI 线程上有一个 SynchronizationContext（WPF 里是 DispatcherSynchronizationContext），
// 它提供了"把一段代码投递回 UI 线程执行"的能力。线程池线程上没有这个上下文。
//
// ConfigureAwait 是 Task / ValueTask 上的一个方法（不是关键字），用来指定 await 之后的续体是否回到 await 之前捕获的上下文上执行。
//   - ConfigureAwait(true)（默认行为，不写就是 true）：await 时捕获当前的 SynchronizationContext，
//     Task 完成后把续体投递回去。在 UI 线程上 await，续体就回到 UI 线程，可以直接访问控件。第 2 步就是靠这个。
//   - ConfigureAwait(false)：不捕获上下文，续体直接在完成 Task 的线程池线程上执行。
//     省掉了切回 UI 线程的开销，但 await 之后不能再直接访问控件，必须用 Invoke / Dispatcher 切回 UI 线程。
//
// 通常的约定：类库代码不关心自己在哪个线程上，await 都写 ConfigureAwait(false)；
// UI 层代码在 await 之后要操作控件，保留默认的 ConfigureAwait(true)。
//
// 点击后对比输出的 ThreadId：true 那一行和 UI ThreadId 相同，false 那一行不同。
// 记住"续体要回到 UI 线程执行"这一点，第 7 步的死锁就是由它引起的。
// -------------------------------------------------------------------------------------
button5.Click += async (sender, args) =>
{
	text.AppendText("UI ThreadId: " + Thread.CurrentThread.ManagedThreadId + "\r\n");
	
	// ConfigureAwait(true)：await 之后回到 UI 线程，可以直接访问控件，ThreadId 和 UI ThreadId 相同
	var result = await Task.Run(() => Fun()).ConfigureAwait(true);
	text.AppendText(result + " ConfigureAwait(true)  ThreadId: " + Thread.CurrentThread.ManagedThreadId + "\r\n");
	
	// ConfigureAwait(false)：await 之后在线程池线程上，ThreadId 不同，直接访问控件会抛跨线程异常，需要 Invoke 切回 UI 线程
	result = await Task.Run(() => Fun()).ConfigureAwait(false);
	int threadId = Thread.CurrentThread.ManagedThreadId;
	panel.GetControl().Invoke(() => { text.AppendText(result + " ConfigureAwait(false) ThreadId: " + threadId + "\r\n"); });
};

// -------------------------------------------------------------------------------------
// 第 6 步：Wait / .Result —— 用同步的方式等待异步任务（sync over async）
//
// 事件处理程序没有 async，没法 await，于是用 task.Result（或 task.Wait()）同步等待。
// .Result 会阻塞调用线程，直到 Task 完成、取消或出错。调用线程是 UI 线程，所以效果和第 1 步一样：UI 冻结 5 秒。
// 这里之所以只是"卡"而不是"死锁"，是因为 Task.Run 里的 Fun() 在线程池线程上执行，完成时不需要 UI 线程参与。
//
// 下一步把 Task.Run(() => Fun()) 换成一个内部有 await 的 async 方法，情况就不一样了。
// -------------------------------------------------------------------------------------
button6.Click += (sender, args) =>
{
	foreach (var element in Enumerable.Range(0, 5))
	{
		var task = Task.Run(() => Fun());
		
		//调用线程阻塞在 Wait 处，直到任务完成、取消或者异常
		//task.Wait();
		text.AppendText(task.Result + "\r\n");
	}
};

// -------------------------------------------------------------------------------------
// 第 7 步：Deadlock（死锁）—— 第 5 步和第 6 步合在一起
//
// 和第 6 步一样在 UI 线程上 .Result 同步等待，区别是等待的对象换成了 async 方法 DeadlockAsync()：
//   ① UI 线程调用 DeadlockAsync()。async 方法本身不会启动新线程，await 之前的代码就在 UI 线程上执行。
//   ② 执行到 await Task.Delay(1000)，Task 还没完成。按第 5 步的默认行为，await 捕获 UI 线程的 SynchronizationContext，
//      然后 DeadlockAsync() 返回一个未完成的 Task。
//   ③ UI 线程接着执行 task.Result，阻塞住，等这个 Task 完成。
//   ④ 1 秒后 Task.Delay 完成，DeadlockAsync 剩下的部分（return "死锁了"）要投递回 UI 线程执行。
//   ⑤ 但 UI 线程正卡在 ③ 的 .Result 上，没法执行续体；续体不执行，Task 就永远完成不了。
// UI 线程等 Task，Task 等 UI 线程，两边互相等待，UI 永久卡死，只能结束 LINQPad 查询。
//
// 注释掉的 Task.Run 写法不会死锁：async lambda 在线程池线程上开始执行，await 捕获不到 UI 上下文，
// 续体在线程池上完成，所以只会像第 6 步一样阻塞 UI 约 1 秒。
// -------------------------------------------------------------------------------------
button7.Click += (sender, args) =>
{
	foreach (var element in Enumerable.Range(0, 5))
	{
		var task = DeadlockAsync();
		
		//var task = Task.Run( async () =>
		//{
		//	await Task.Delay(1000);
		//	return "不会死锁";
		//});
		
		//task.Wait();
		text.AppendText(task.Result + "\r\n");
	}
};

// -------------------------------------------------------------------------------------
// 第 8 步：Deadlock + ConfigureAwait(false) —— 用第 5 步的 false 解开死锁
//
// 和第 7 步完全一样在 UI 线程上 .Result 同步等待，唯一的区别是 NoDeadlockAsync 内部写了 ConfigureAwait(false)。
// 第 7 步 ④ 那里，续体不再需要回到 UI 线程，而是在线程池线程上执行并完成 Task，于是 ③ 的 .Result 能够返回。
// 这也是第 5 步提到的类库代码约定的由来：类库的 await 都写 ConfigureAwait(false)，
// 即使调用方错误地用 .Result 同步等待，也不会因为类库内部的 await 而死锁。
// HttpClient 等 .NET 内置库就是这么做的，见同目录的 HttpClientNotReturn.linq。
//
// 但这只是治标：UI 线程仍然被 .Result 阻塞，界面还是冻结约 5 秒，问题退回到了第 6 步。
// 根本的做法是回到第 2、3 步：事件处理程序写成 async，async/await 一路到底，不在 UI 线程上 .Result / .Wait()。
// -------------------------------------------------------------------------------------
button8.Click += (sender, args) =>
{
	foreach (var element in Enumerable.Range(0, 5))
	{
		var task = NoDeadlockAsync();
		text.AppendText(task.Result + "\r\n");
	}
};

// 第 7 步使用：await 默认捕获 UI 上下文，续体必须回到 UI 线程才能完成
async Task<string> DeadlockAsync()
{
	await Task.Delay(1000);
	return "死锁了";
}

// 第 8 步使用：ConfigureAwait(false) 不捕获上下文，续体在线程池线程上完成
async Task<string> NoDeadlockAsync()
{
	await Task.Delay(1000).ConfigureAwait(false);
	return "ConfigureAwait(false)，没有死锁";
}

// 第 2 步注释中的写法：真正的异步等待，等待期间不占用线程
async Task<string> FunAsync()
{
	await Task.Delay(1000);
	return "call function";
}

// 模拟耗时的同步操作：阻塞当前线程 1 秒
string Fun()
{
	Thread.Sleep(1000);
	return "call function";
}
